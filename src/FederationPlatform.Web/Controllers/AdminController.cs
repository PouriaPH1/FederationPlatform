using FederationPlatform.Application.Interfaces;
using FederationPlatform.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly IActivityService _activityService;
    private readonly INewsService _newsService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUserService userService,
        IActivityService activityService,
        INewsService newsService,
        ILogger<AdminController> logger)
    {
        _userService = userService;
        _activityService = activityService;
        _newsService = newsService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // Redirect to the comprehensive dashboard
        return RedirectToAction("Dashboard", "Dashboard");
    }

    public async Task<IActionResult> Users(string? searchTerm = null, string? role = null)
    {
        try
        {
            var allUsers = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(searchTerm))
                allUsers = allUsers?.Where(u => 
                    u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            var model = new DashboardViewModel
            {
                AdminDashboard = new AdminDashboardViewModel
                {
                    AllUsers = allUsers?.Select(u => new UserAdminViewModel
                    {
                        Id = u.Id,
                        FullName = u.FirstName + " " + u.LastName,
                        Email = u.Email,
                        UniversityName = "دانشگاه",
                        RegisterDate = u.CreatedAt
                    }).ToList() ?? new List<UserAdminViewModel>()
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            return BadRequest("خطایی در بارگذاری کاربران رخ داد");
        }
    }

    public async Task<IActionResult> PendingActivities()
    {
        try
        {
            var pendingActivities = await _activityService.GetPendingActivitiesAsync();

            var model = new DashboardViewModel
            {
                AdminDashboard = new AdminDashboardViewModel
                {
                    PendingActivitiesCount = pendingActivities?.Count() ?? 0,
                    PendingActivities = pendingActivities?.Select(a => new PendingActivityViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        UniversityName = a.University?.Name ?? "",
                        RepresentativeName = a.Representative?.UserName ?? "",
                        SubmitDate = a.CreatedAt,
                        StartDate = a.StartDate,
                        Location = a.Location,
                        ExpectedParticipants = a.ParticipantCount,
                        Category = a.Category
                    }).ToList() ?? new List<PendingActivityViewModel>()
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending activities");
            return BadRequest("خطایی در بارگذاری فعالیت‌های درانتظار رخ داد");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ApproveActivity(int activityId)
    {
        try
        {
            var result = await _activityService.ApproveActivityAsync(activityId);
            
            if (!result.Succeeded)
                return BadRequest(result.Messages?.FirstOrDefault() ?? "خطا در تایید");

            _logger.LogInformation("Activity {ActivityId} approved", activityId);
            return RedirectToAction("PendingActivities", new { message = "فعالیت تایید شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving activity");
            return BadRequest("خطایی در حین تایید رخ داد");
        }
    }

    [HttpPost]
    public async Task<IActionResult> RejectActivity(int activityId, string rejectionReason)
    {
        try
        {
            var result = await _activityService.RejectActivityAsync(activityId, rejectionReason);
            
            if (!result.Succeeded)
                return BadRequest(result.Messages?.FirstOrDefault() ?? "خطا در رد کردن");

            _logger.LogInformation("Activity {ActivityId} rejected", activityId);
            return RedirectToAction("PendingActivities", new { message = "فعالیت رد شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting activity");
            return BadRequest("خطایی در حین رد کردن رخ داد");
        }
    }

    public async Task<IActionResult> Statistics()
    {
        try
        {
            var stats = await _activityService.GetActivityStatisticsAsync();

            var model = new DashboardViewModel
            {
                AdminDashboard = new AdminDashboardViewModel
                {
                    Statistics = stats?.Select(s => new ActivityStatViewModel
                    {
                        Category = s.Category,
                        Count = s.Count
                    }).ToList() ?? new List<ActivityStatViewModel>()
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading statistics");
            return BadRequest("خطایی در بارگذاری آمار رخ داد");
        }
    }

    public async Task<IActionResult> ManageNews()
    {
        try
        {
            var news = await _newsService.GetAllNewsAsync();

            var model = new DashboardViewModel
            {
                AdminDashboard = new AdminDashboardViewModel
                {
                    ManageNews = news?.Select(n => new ManageNewsViewModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Content = n.Content,
                        PublishDate = n.PublishDate,
                        UniversityName = n.University?.Name ?? "عمومی"
                    }).ToList() ?? new List<ManageNewsViewModel>()
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news management");
            return BadRequest("خطایی در بارگذاری مدیریت اخبار رخ داد");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(userId);
            
            if (!result.Succeeded)
                return BadRequest(result.Messages?.FirstOrDefault() ?? "خطا در حذف");

            _logger.LogInformation("User {UserId} deleted", userId);
            return RedirectToAction("Users", new { message = "کاربر حذف شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return BadRequest("خطایی در حین حذف رخ داد");
        }
    }
}
