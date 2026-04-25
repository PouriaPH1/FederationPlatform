using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FederationPlatform.Web.Controllers;

[Authorize]
public class FeedbackController : Controller
{
    private readonly IFeedbackService _feedbackService;
    private readonly IActivityService _activityService;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(
        IFeedbackService feedbackService,
        IActivityService activityService,
        ILogger<FeedbackController> logger)
    {
        _feedbackService = feedbackService;
        _activityService = activityService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : 0;
    }

    [HttpGet]
    public async Task<IActionResult> ActivityFeedbacks(int activityId)
    {
        try
        {
            var activity = await _activityService.GetActivityByIdAsync(activityId);
            if (activity == null)
                return NotFound();

            var feedbacks = await _feedbackService.GetApprovedFeedbacksAsync(activityId);
            var averageRating = await _feedbackService.GetAverageRatingAsync(activityId);

            ViewBag.Activity = activity;
            ViewBag.AverageRating = averageRating;

            return View(feedbacks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedbacks");
            TempData["Error"] = "خطا در بارگیری نظرات";
            return RedirectToAction("Index", "Activity");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(int activityId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var activity = await _activityService.GetActivityByIdAsync(activityId);
            if (activity == null)
                return NotFound();

            // Check if user already has feedback
            if (await _feedbackService.UserHasFeedbackAsync(userId, activityId))
            {
                TempData["Error"] = "شما قبلاً برای این فعالیت نظر ثبت کرده‌اید";
                return RedirectToAction("ActivityFeedbacks", new { activityId });
            }

            ViewBag.Activity = activity;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback form");
            TempData["Error"] = "خطا در بارگیری فرم";
            return RedirectToAction("Index", "Activity");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int activityId, int rating, string comment)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var dto = new CreateFeedbackDto
            {
                ActivityId = activityId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };

            await _feedbackService.CreateFeedbackAsync(dto);
            TempData["Success"] = "نظر شما با موفقیت ثبت شد و پس از تایید نمایش داده خواهد شد";
            return RedirectToAction("ActivityFeedbacks", new { activityId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("ActivityFeedbacks", new { activityId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating feedback");
            TempData["Error"] = "خطا در ثبت نظر";
            return RedirectToAction("Create", new { activityId });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Pending()
    {
        try
        {
            var allFeedbacks = await _feedbackService.GetActivityFeedbacksAsync(0);
            var pendingFeedbacks = allFeedbacks.Where(f => !f.IsApproved).ToList();
            return View(pendingFeedbacks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending feedbacks");
            TempData["Error"] = "خطا در بارگیری نظرات";
            return RedirectToAction("Index", "Admin");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var result = await _feedbackService.ApproveFeedbackAsync(id);
            if (result)
            {
                TempData["Success"] = "نظر تایید شد";
            }
            else
            {
                TempData["Error"] = "نظر یافت نشد";
            }

            return RedirectToAction("Pending");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving feedback");
            TempData["Error"] = "خطا در تایید نظر";
            return RedirectToAction("Pending");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _feedbackService.DeleteFeedbackAsync(id);
            if (result)
            {
                TempData["Success"] = "نظر حذف شد";
            }
            else
            {
                TempData["Error"] = "نظر یافت نشد";
            }

            return RedirectToAction("Pending");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting feedback");
            TempData["Error"] = "خطا در حذف نظر";
            return RedirectToAction("Pending");
        }
    }
}
