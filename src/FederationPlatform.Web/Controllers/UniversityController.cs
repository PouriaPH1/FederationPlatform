using FederationPlatform.Application.Interfaces;
using FederationPlatform.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

public class UniversityController : Controller
{
    private readonly IUniversityService _universityService;
    private readonly IActivityService _activityService;
    private readonly IUserService _userService;
    private readonly INewsService _newsService;
    private readonly ILogger<UniversityController> _logger;

    public UniversityController(
        IUniversityService universityService,
        IActivityService activityService,
        IUserService userService,
        INewsService newsService,
        ILogger<UniversityController> logger)
    {
        _universityService = universityService;
        _activityService = activityService;
        _userService = userService;
        _newsService = newsService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null, string? sortBy = null)
    {
        try
        {
            var pageSize = 12;
            var universities = await _universityService.GetAllUniversitiesAsync();

            if (!string.IsNullOrEmpty(searchTerm))
                universities = universities?.Where(u => 
                    u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (sortBy == "city")
                universities = universities?.OrderBy(u => u.City).ToList();
            else if (sortBy == "activities")
                universities = universities?.OrderByDescending(u => u.Activities?.Count ?? 0).ToList();
            else
                universities = universities?.OrderBy(u => u.Name).ToList();

            var totalCount = universities?.Count() ?? 0;
            var paginatedUniversities = universities
                ?.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<UniversityDto>();

            var model = new UniversityListViewModel
            {
                Universities = paginatedUniversities.Select(u => new UniversityCardViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    City = u.City,
                    ActivitiesCount = u.Activities?.Count ?? 0,
                    MembersCount = u.Representatives?.Count ?? 0,
                    NewsCount = 0
                }).ToList(),
                CurrentPage = page,
                TotalPages = (totalCount + pageSize - 1) / pageSize,
                SearchTerm = searchTerm ?? ""
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading universities");
            return BadRequest("خطایی در بارگذاری دانشگاه‌ها رخ داد");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var university = await _universityService.GetUniversityByIdAsync(id);
            if (university == null)
                return NotFound();

            var activities = await _activityService.GetUniversityActivitiesAsync(id);
            var news = await _newsService.GetUniversityNewsAsync(id);

            var model = new UniversityDetailViewModel
            {
                Id = university.Id,
                Name = university.Name,
                City = university.City,
                Phone = university.Phone ?? "",
                Description = university.Description ?? "",
                ActivitiesCount = activities?.Count() ?? 0,
                MembersCount = university.Representatives?.Count ?? 0,
                NewsCount = news?.Count() ?? 0,
                WorkshopsCount = activities?.Count(a => a.Category == "Workshop") ?? 0,
                RecentActivities = activities?.Take(6).Select(a => new ActivityBriefViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Status = a.IsApproved ? "تایید شده" : "در انتظار",
                    StartDate = a.StartDate,
                    Location = a.Location
                }).ToList() ?? new List<ActivityBriefViewModel>(),
                Representatives = university.Representatives?.Select(r => new UserBriefViewModel
                {
                    Id = r.Id,
                    FullName = r.FirstName + " " + r.LastName,
                    Email = r.Email,
                    Phone = r.UserProfile?.Phone ?? ""
                }).ToList() ?? new List<UserBriefViewModel>(),
                News = news?.Select(n => new NewsItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    PublishDate = n.PublishDate
                }).ToList() ?? new List<NewsItemViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading university details");
            return BadRequest("خطایی در بارگذاری جزئیات دانشگاه رخ داد");
        }
    }
}
