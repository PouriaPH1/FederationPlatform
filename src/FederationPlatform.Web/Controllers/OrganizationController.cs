using FederationPlatform.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

public class OrganizationController : Controller
{
    private readonly IOrganizationService _organizationService;
    private readonly IActivityService _activityService;
    private readonly ILogger<OrganizationController> _logger;

    public OrganizationController(
        IOrganizationService organizationService,
        IActivityService activityService,
        ILogger<OrganizationController> logger)
    {
        _organizationService = organizationService;
        _activityService = activityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var organizations = await _organizationService.GetAllOrganizationsAsync();
            return View(organizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading organizations");
            TempData["Error"] = "خطا در بارگیری تشکل‌ها";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var organization = await _organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
                return NotFound();

            // Get activities for this organization
            var allActivities = await _activityService.GetAllActivitiesAsync();
            var orgActivities = allActivities.Where(a => a.OrganizationId == id).ToList();

            ViewBag.Activities = orgActivities;
            return View(organization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading organization details");
            TempData["Error"] = "خطا در بارگیری جزئیات تشکل";
            return RedirectToAction("Index");
        }
    }
}
