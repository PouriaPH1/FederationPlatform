using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Services;
using FederationPlatform.Web.Models;
using FederationPlatform.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FederationPlatform.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IUserService _userService;
    private readonly IActivityService _activityService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IUserService userService,
        IActivityService activityService,
        ILogger<DashboardController> logger)
    {
        _userService = userService;
        _activityService = activityService;
        _logger = logger;
    }

    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

            if (userRole == "Admin")
                return await AdminDashboard();

            if (userRole == "Representative")
                return await RepresentativeDashboard(userId);

            return await UserDashboard(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return BadRequest("خطایی در بارگذاری داشبورد رخ داد");
        }
    }

    private async Task<IActionResult> UserDashboard(string userId)
    {
        try
        {
            if (!int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userIdInt);
            var activities = await _activityService.GetUserActivitiesAsync(userIdInt);

            var model = new DashboardViewModel
            {
                UserName = user?.FirstName + " " + user?.LastName ?? "کاربر",
                UserDashboard = new UserDashboardViewModel
                {
                    MyActivitiesCount = activities?.Count(a => a.RepresentativeId == userIdInt) ?? 0,
                    ApprovedActivitiesCount = activities?.Count(a => a.IsApproved) ?? 0,
                    PendingActivitiesCount = activities?.Count(a => !a.IsApproved) ?? 0,
                    JoinedActivitiesCount = 0,
                    UserRole = "User",
                    MyActivities = activities?.Take(5).Select(a => new ActivityBriefViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Status = a.Status,
                        StartDate = a.StartDate,
                        Location = a.Location,
                        UniversityName = a.UniversityName,
                        Description = a.Description
                    }).ToList() ?? new List<ActivityBriefViewModel>(),
                    JoinedActivities = new List<ActivityDetailViewModel>()
                }
            };

            return View("UserDashboard", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user dashboard");
            throw;
        }
    }

    private async Task<IActionResult> RepresentativeDashboard(string userId)
    {
        try
        {
            if (!int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userIdInt);
            var activities = await _activityService.GetUserActivitiesAsync(userIdInt);

            var model = new DashboardViewModel
            {
                UserName = user?.FirstName + " " + user?.LastName ?? "نماینده",
                UserDashboard = new UserDashboardViewModel
                {
                    MyActivitiesCount = activities?.Count() ?? 0,
                    ApprovedActivitiesCount = activities?.Count(a => a.IsApproved) ?? 0,
                    PendingActivitiesCount = activities?.Count(a => !a.IsApproved) ?? 0,
                    JoinedActivitiesCount = 0,
                    UserRole = "Representative",
                    MyActivities = activities?.Select(a => new ActivityBriefViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Status = a.Status,
                        StartDate = a.StartDate,
                        Location = a.Location,
                        UniversityName = a.UniversityName,
                        Description = a.Description
                    }).ToList() ?? new List<ActivityBriefViewModel>(),
                    JoinedActivities = new List<ActivityDetailViewModel>()
                }
            };

            return View("UserDashboard", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading representative dashboard");
            throw;
        }
    }

    [Authorize(Roles = "Admin")]
    private async Task<IActionResult> AdminDashboard()
    {
        try
        {
            var totalUsers = await _userService.GetTotalUsersCountAsync();
            var totalActivities = await _activityService.GetTotalActivitiesCountAsync();
            var pendingActivities = await _activityService.GetPendingActivitiesAsync();
            var recentUsers = await _userService.GetRecentUsersAsync(5);

            var model = new DashboardViewModel
            {
                UserName = "مدیر سیستم",
                AdminDashboard = new AdminDashboardViewModel
                {
                    TotalUsers = totalUsers,
                    TotalActivities = totalActivities,
                    TotalUniversities = 25, // From seed data
                    PendingActivitiesCount = pendingActivities?.Count() ?? 0,
                    PendingActivityList = pendingActivities?.Take(5).Select(a => new PendingActivityViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description ?? "",
                        UniversityName = a.University ?? "",
                        RepresentativeName = a.Representative ?? "",
                        SubmitDate = a.CreatedAt,
                        StartDate = a.StartDate,
                        Location = a.Location,
                        ExpectedParticipants = a.ParticipantCount ?? 0,
                        Category = a.Category,
                        Files = a.Files?.Select(f => new ActivityFileViewModel
                        {
                            Id = f.Id,
                            FileName = f.FileName,
                            Url = f.FilePath
                        }).ToList() ?? new List<ActivityFileViewModel>()
                    }).ToList() ?? new List<PendingActivityViewModel>(),
                    RecentUsers = recentUsers?.Select(u => new UserAdminViewModel
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        UniversityName = u.UniversityId > 0 ? "دانشگاه" : "نامشخص",
                        RegisterDate = u.CreatedAt
                    }).ToList() ?? new List<UserAdminViewModel>()
                }
            };

            return View("AdminDashboard", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            throw;
        }
    }
}
