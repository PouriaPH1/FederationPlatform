using FederationPlatform.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ActivityLogController : Controller
{
    private readonly IActivityLogService _activityLogService;
    private readonly ILogger<ActivityLogController> _logger;

    public ActivityLogController(IActivityLogService activityLogService, ILogger<ActivityLogController> logger)
    {
        _activityLogService = activityLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
    {
        try
        {
            var logs = await _activityLogService.GetLogsAsync(page, pageSize);
            var totalCount = await _activityLogService.GetTotalLogsCountAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity logs");
            TempData["Error"] = "خطا در بارگیری لاگ‌ها";
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpGet]
    public async Task<IActionResult> UserLogs(int userId, int page = 1, int pageSize = 50)
    {
        try
        {
            var logs = await _activityLogService.GetUserLogsAsync(userId, page, pageSize);
            ViewBag.UserId = userId;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View("Index", logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user logs");
            TempData["Error"] = "خطا در بارگیری لاگ‌های کاربر";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EntityLogs(string entityType, int entityId)
    {
        try
        {
            var logs = await _activityLogService.GetEntityLogsAsync(entityType, entityId);
            ViewBag.EntityType = entityType;
            ViewBag.EntityId = entityId;

            return View("Index", logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading entity logs");
            TempData["Error"] = "خطا در بارگیری لاگ‌های موجودیت";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CleanupOldLogs(int daysToKeep = 90)
    {
        try
        {
            await _activityLogService.DeleteOldLogsAsync(daysToKeep);
            TempData["Success"] = $"لاگ‌های قدیمی‌تر از {daysToKeep} روز حذف شدند";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old logs");
            TempData["Error"] = "خطا در پاکسازی لاگ‌های قدیمی";
            return RedirectToAction("Index");
        }
    }
}
