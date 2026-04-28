using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Services;
using FederationPlatform.Domain.Enums;
using FederationPlatform.Web.Models;
using FederationPlatform.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly IActivityService _activityService;
    private readonly INewsService _newsService;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IActivityLogService _activityLogService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUserService userService,
        IActivityService activityService,
        INewsService newsService,
        INotificationService notificationService,
        IEmailService emailService,
        IActivityLogService activityLogService,
        ILogger<AdminController> logger)
    {
        _userService = userService;
        _activityService = activityService;
        _newsService = newsService;
        _notificationService = notificationService;
        _emailService = emailService;
        _activityLogService = activityLogService;
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
                        Username = u.UserName,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.Role.ToString(),
                        IsActive = u.IsActive,
                        UniversityName = "دانشگاه",
                        RegisterDate = u.CreatedAt,
                        CreatedAt = u.CreatedAt
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
                    PendingActivityList = pendingActivities?.Select(a => new PendingActivityViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        UniversityName = a.UniversityName ?? "",
                        RepresentativeName = a.Representative ?? "",
                        SubmitDate = a.CreatedAt,
                        StartDate = a.StartDate,
                        Location = a.Location,
                        ExpectedParticipants = a.ParticipantCount ?? 0,
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
            // Get activity details before approval
            var activity = await _activityService.GetActivityByIdAsync(activityId);
            if (activity == null)
                return NotFound();

            var result = await _activityService.ApproveActivityAsync(activityId);
            
            if (!result)
                return BadRequest("خطا در تایید");

            // Send notification to representative
            await _notificationService.SendActivityApprovalNotificationAsync(
                activity.UserId, activityId, activity.Title);

            // Send email to representative
            var user = await _userService.GetUserByIdAsync(activity.UserId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendActivityApprovalEmailAsync(
                    user.Email, user.Username, activity.Title);
            }

            // Log the action
            await _activityLogService.LogAsync(new Application.DTOs.CreateActivityLogDto
            {
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Action = "Approve",
                EntityType = "Activity",
                EntityId = activityId,
                Details = $"تایید فعالیت: {activity.Title}",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });

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
            // Get activity details before rejection
            var activity = await _activityService.GetActivityByIdAsync(activityId);
            if (activity == null)
                return NotFound();

            var result = await _activityService.RejectActivityAsync(activityId);
            
            if (!result)
                return BadRequest("خطا در رد کردن");

            // Send notification to representative
            await _notificationService.SendActivityRejectionNotificationAsync(
                activity.UserId, activityId, activity.Title);

            // Send email to representative
            var user = await _userService.GetUserByIdAsync(activity.UserId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendActivityRejectionEmailAsync(
                    user.Email, user.Username, activity.Title, rejectionReason);
            }

            // Log the action
            await _activityLogService.LogAsync(new Application.DTOs.CreateActivityLogDto
            {
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Action = "Reject",
                EntityType = "Activity",
                EntityId = activityId,
                Details = $"رد فعالیت: {activity.Title} - دلیل: {rejectionReason}",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });

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
                    ManageNews = new ManageNewsViewModel
                    {
                        NewsList = news?.Select(n => new NewsItemViewModel
                        {
                            Id = n.Id,
                            Title = n.Title,
                            Content = n.Content,
                            ImageUrl = n.ImageUrl,
                            PublishedAt = n.PublishedAt,
                            CreatedByName = n.CreatedByUsername
                        }).ToList() ?? new List<NewsItemViewModel>(),
                        TotalCount = news?.Count() ?? 0
                    }
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
            if (!int.TryParse(userId, out int id))
                return BadRequest("شناسه کاربر معتبر نیست");

            var result = await _userService.DeleteUserAsync(id);
            
            if (!result)
                return BadRequest("خطا در حذف");

            // Log the action
            await _activityLogService.LogAsync(new Application.DTOs.CreateActivityLogDto
            {
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Action = "Delete",
                EntityType = "User",
                EntityId = id,
                Details = $"حذف کاربر: {id}",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });

            _logger.LogInformation("User {UserId} deleted", id);
            return RedirectToAction("Users", new { message = "کاربر حذف شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return BadRequest("خطایی در حین حذف رخ داد");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BanUser(string userId)
    {
        try
        {
            if (!int.TryParse(userId, out int id))
                return BadRequest("شناسه کاربر معتبر نیست");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("کاربر یافت نشد");

            var result = await _userService.BanUserAsync(id);
            
            if (!result)
                return BadRequest("خطا در مسدود کردن کاربر");

            // Send notification to user
            await _notificationService.SendUserBannedNotificationAsync(id, "حساب شما توسط مدیر سیستم مسدود شد");

            // Send email to user
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendUserBannedEmailAsync(
                    user.Email, 
                    user.Username, 
                    "حساب شما مسدود شد");
            }

            // Log the action
            await _activityLogService.LogAsync(new Application.DTOs.CreateActivityLogDto
            {
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Action = "Ban",
                EntityType = "User",
                EntityId = id,
                Details = $"مسدود کردن کاربر: {user.Username} ({user.Email})",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });

            _logger.LogInformation("User {UserId} banned by admin", id);
            return RedirectToAction("Users", new { message = "کاربر مسدود شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error banning user");
            return BadRequest("خطایی در حین مسدود کردن رخ داد");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        try
        {
            if (!int.TryParse(userId, out int id))
                return BadRequest("شناسه کاربر معتبر نیست");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("کاربر یافت نشد");

            var result = await _userService.ActivateUserAsync(id);
            
            if (!result)
                return BadRequest("خطا در فعال کردن کاربر");

            // Send notification to user
            await _notificationService.SendUserActivatedNotificationAsync(id, "حساب شما فعال شد");

            // Send email to user
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendUserActivatedEmailAsync(
                    user.Email, 
                    user.Username);
            }

            // Log the action
            await _activityLogService.LogAsync(new Application.DTOs.CreateActivityLogDto
            {
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Action = "Activate",
                EntityType = "User",
                EntityId = id,
                Details = $"فعال کردن کاربر: {user.Username} ({user.Email})",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });

            _logger.LogInformation("User {UserId} activated by admin", id);
            return RedirectToAction("Users", new { message = "کاربر فعال شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user");
            return BadRequest("خطایی در حین فعال کردن رخ داد");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PromoteToRepresentative(string userId)
    {
        try
        {
            if (!int.TryParse(userId, out int id))
                return BadRequest("شناسه کاربر معتبر نیست");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("کاربر یافت نشد");

            if (user.Role == UserRole.Representative || user.Role == UserRole.Admin)
                return BadRequest("این کاربر قبلاً نماینده یا مدیر است");

            var result = await _userService.PromoteToRepresentativeAsync(id);
            
            if (!result)
                return BadRequest("خطا در ترفیع کاربر");

            // Send notification to user
            await _notificationService.SendUserPromotedNotificationAsync(
                id, 
                "شما به نماینده ارتقا یافتید",
                "شما اکنون می‌توانید فعالیت‌ها را ایجاد و ارسال کنید");

            // Send email to user
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendUserPromotedEmailAsync(
                    user.Email, 
                    user.Username, 
                    "ترفیع به نماینده");
            }

            // Log the action
            await _activityLogService.LogAsync(new Application.DTOs.CreateActivityLogDto
            {
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Action = "Promote",
                EntityType = "User",
                EntityId = id,
                Details = $"ترفیع کاربر {user.Username} ({user.Email}) به نماینده",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });

            _logger.LogInformation("User {UserId} promoted to Representative by admin", id);
            return RedirectToAction("Users", new { message = "کاربر به نماینده ترفیع یافت" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error promoting user");
            return BadRequest("خطایی در حین ترفیع رخ داد");
        }
    }
}
