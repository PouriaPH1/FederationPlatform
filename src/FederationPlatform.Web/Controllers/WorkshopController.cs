using FederationPlatform.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

public class WorkshopController : Controller
{
    private readonly IWorkshopService _workshopService;
    private readonly ILogger<WorkshopController> _logger;

    public WorkshopController(IWorkshopService workshopService, ILogger<WorkshopController> logger)
    {
        _workshopService = workshopService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        try
        {
            var allWorkshops = await _workshopService.GetAllWorkshopsAsync();
            var totalCount = allWorkshops.Count();
            var workshops = allWorkshops
                .OrderByDescending(w => w.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(workshops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading workshops");
            TempData["Error"] = "خطا در بارگیری کارگاه‌ها";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var workshop = await _workshopService.GetWorkshopByIdAsync(id);
            if (workshop == null)
                return NotFound();

            return View(workshop);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading workshop details");
            TempData["Error"] = "خطا در بارگیری جزئیات کارگاه";
            return RedirectToAction("Index");
        }
    }
}
