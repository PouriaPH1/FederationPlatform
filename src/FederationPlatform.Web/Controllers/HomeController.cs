using FederationPlatform.Application.Services;
using FederationPlatform.Web.Models;
using FederationPlatform.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

public class HomeController : Controller
{
    private readonly IActivityService _activityService;
    private readonly IUniversityService _universityService;
    private readonly INewsService _newsService;
    private readonly IUserService _userService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IActivityService activityService,
        IUniversityService universityService,
        INewsService newsService,
        IUserService userService,
        ILogger<HomeController> logger)
    {
        _activityService = activityService;
        _universityService = universityService;
        _newsService = newsService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var universities = await _universityService.GetAllUniversitiesAsync();
            var activities = await _activityService.GetApprovedActivitiesAsync();
            var news = await _newsService.GetLatestNewsAsync(5);
            var userCount = await _userService.GetTotalUsersCountAsync();

            var model = new HomeViewModel
            {
                UniversitiesCount = universities?.Count() ?? 0,
                ActivitiesCount = activities?.Count() ?? 0,
                UsersCount = userCount,
                NewsCount = news?.Count() ?? 0,
                Universities = universities?.Take(6).Select(u => new UniversityBriefViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    City = u.City,
                    ActivityCount = 0
                }).ToList() ?? new List<UniversityBriefViewModel>(),
                News = news?.Select(n => new NewsItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    PublishDate = n.PublishDate
                }).ToList() ?? new List<NewsItemViewModel>(),
                RecentActivities = activities?.Select(a => new ActivityBriefViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    StartDate = a.StartDate,
                    Location = a.Location,
                    UniversityName = a.UniversityName,
                    Status = a.Status
                }).ToList() ?? new List<ActivityBriefViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            return View(new HomeViewModel());
        }
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
