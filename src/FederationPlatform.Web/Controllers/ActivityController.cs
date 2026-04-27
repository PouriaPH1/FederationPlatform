using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Services;
using FederationPlatform.Web.Models;
using FederationPlatform.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FederationPlatform.Web.Controllers;

[Authorize]
public class ActivityController : Controller
{
    private readonly IActivityService _activityService;
    private readonly IUniversityService _universityService;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(
        IActivityService activityService,
        IUniversityService universityService,
        IFileService fileService,
        INotificationService notificationService,
        IEmailService emailService,
        IUserService userService,
        ILogger<ActivityController> logger)
    {
        _activityService = activityService;
        _universityService = universityService;
        _fileService = fileService;
        _notificationService = notificationService;
        _emailService = emailService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
    {
        try
        {
            var activities = await _activityService.GetApprovedActivitiesAsync();

            var model = new ActivityListViewModel
            {
                Activities = activities?.Select(a => new ActivityDetailViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    StartDate = a.StartDate,
                    IsApproved = a.IsApproved
                }).ToList() ?? new List<ActivityDetailViewModel>(),
                CurrentPage = page,
                TotalPages = 1,
                SearchTerm = searchTerm ?? ""
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activities");
            return BadRequest("خطایی در بارگذاری فعالیت‌ها رخ داد");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
                return NotFound();

            var model = new ActivityDetailViewModel
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Location = activity.Location,
                StartDate = activity.StartDate,
                EndDate = activity.EndDate,
                UniversityName = activity.UniversityName ?? "",
                RepresentativeName = activity.Representative ?? "",
                ParticipantsCount = activity.ParticipantCount,
                IsApproved = activity.IsApproved,
                Files = activity.Files?.Select(f => new ActivityFileViewModel
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    Url = f.FilePath
                }).ToList() ?? new List<ActivityFileViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity details");
            return BadRequest("خطایی در بارگذاری جزئیات فعالیت رخ داد");
        }
    }

    [Authorize(Roles = "Representative,Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var universities = await _universityService.GetAllUniversitiesAsync();
            
            var model = new CreateActivityViewModel
            {
                Universities = universities?.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                }).ToList() ?? new List<SelectListItem>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create activity form");
            return BadRequest("خطایی در بارگذاری فرم رخ داد");
        }
    }

    [Authorize(Roles = "Representative,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateActivityViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var createDto = new CreateActivityDto
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UniversityId = model.UniversityId,
                Category = model.Category,
                ExpectedParticipants = model.ExpectedParticipants ?? 0,
                Budget = model.Budget ?? 0
            };

            if (!int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var result = await _activityService.CreateActivityAsync(userIdInt, createDto);
            
            if (result == null || result.Id == 0)
                return BadRequest("خطا در ایجاد فعالیت");

            // Handle file uploads
            if (model.Files?.Any() ?? false)
            {
                foreach (var file in model.Files)
                {
                    if (file.Length > 0)
                    {
                        var uploadResult = await _fileService.UploadFileAsync(file, $"activities/{result.Id}");
                        if (string.IsNullOrEmpty(uploadResult))
                            _logger.LogWarning("Failed to upload file: {FileName}", file.FileName);
                    }
                }
            }

            // Send notifications to all admins
            var activity = await _activityService.GetActivityByIdAsync(result.Id);
            if (activity != null)
            {
                var currentUser = await _userService.GetUserByIdAsync(userIdInt);
                var representativeName = currentUser?.Username ?? "نماینده";

                await _notificationService.SendNewActivityNotificationToAdminAsync(
                    result.Id, activity.Title, representativeName);

                // Send email to admins
                var admins = await _userService.GetAdminUsersAsync();
                foreach (var admin in admins)
                {
                    await _emailService.SendNewActivityNotificationEmailAsync(
                        admin.Email, admin.Username, activity.Title, representativeName);
                }
            }

            _logger.LogInformation("Activity {ActivityId} created successfully", result.Id);
            return RedirectToAction("Details", new { id = result.Id, message = "فعالیت با موفقیت ایجاد شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating activity");
            return BadRequest("خطایی در حین ایجاد فعالیت رخ داد");
        }
    }

    [Authorize(Roles = "Representative,Admin")]
    [HttpGet]
    public async Task<IActionResult> MyActivities()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var activities = await _activityService.GetUserActivitiesAsync(userIdInt);

            var model = new MyActivitiesViewModel
            {
                Activities = activities?.Select(a => new ActivityCardViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Status = a.StatusName,
                    UniversityName = a.UniversityName ?? "",
                    StartDate = a.StartDate,
                    ParticipantsCount = a.ParticipantCount ?? 0
                }).ToList() ?? new List<ActivityCardViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user activities");
            return BadRequest("خطایی در بارگذاری فعالیت‌های شما رخ داد");
        }
    }
}
