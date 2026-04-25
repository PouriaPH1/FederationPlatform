using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FederationPlatform.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class ReportController : Controller
{
    private readonly IReportingService _reportingService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportingService reportingService, ILogger<ReportController> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    [HttpGet("activity-report")]
    public async Task<IActionResult> ActivityReport(int? universityId = null, string? status = null)
    {
        try
        {
            var filter = new ReportFilterDto
            {
                UniversityId = universityId,
                Status = status
            };

            var activities = await _reportingService.GetActivityReportAsync(filter);
            return View("ActivityReport", activities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity report");
            TempData["Error"] = "خطا در بارگیری گزارش";
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpPost("export-activities-excel")]
    public async Task<IActionResult> ExportActivitiesExcel(int? universityId = null, string? status = null)
    {
        try
        {
            var filter = new ReportFilterDto
            {
                UniversityId = universityId,
                Status = status
            };

            var activities = await _reportingService.GetActivityReportAsync(filter);
            var excelFile = await _reportingService.GenerateActivityExcelReportAsync(
                activities,
                "گزارش فعالیت‌ها");

            return File(excelFile, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Activity_Report_{DateTime.Now:yyyy_MM_dd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting activities");
            TempData["Error"] = "خطا در خروجی گزارش";
            return RedirectToAction("ActivityReport");
        }
    }

    [HttpGet("university-report")]
    public async Task<IActionResult> UniversityReport()
    {
        try
        {
            var filter = new ReportFilterDto();
            var universities = await _reportingService.GetUniversityReportAsync(filter);
            return View("UniversityReport", universities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading university report");
            TempData["Error"] = "خطا در بارگیری گزارش";
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpPost("export-universities-excel")]
    public async Task<IActionResult> ExportUniversitiesExcel()
    {
        try
        {
            var filter = new ReportFilterDto();
            var universities = await _reportingService.GetUniversityReportAsync(filter);
            var excelFile = await _reportingService.GenerateUniversityExcelReportAsync(
                universities,
                "گزارش دانشگاه‌ها");

            return File(excelFile,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"University_Report_{DateTime.Now:yyyy_MM_dd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting universities");
            TempData["Error"] = "خطا در خروجی گزارش";
            return RedirectToAction("UniversityReport");
        }
    }

    [HttpGet("representative-report")]
    public async Task<IActionResult> RepresentativeReport(int? universityId = null)
    {
        try
        {
            var representatives = await _reportingService.GetRepresentativeReportAsync(universityId);
            return View("RepresentativeReport", representatives);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading representative report");
            TempData["Error"] = "خطا در بارگیری گزارش";
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpPost("export-representatives-excel")]
    public async Task<IActionResult> ExportRepresentativesExcel(int? universityId = null)
    {
        try
        {
            var representatives = await _reportingService.GetRepresentativeReportAsync(universityId);
            var excelFile = await _reportingService.GenerateRepresentativeExcelReportAsync(
                representatives,
                "گزارش نمایندگان");

            return File(excelFile,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Representative_Report_{DateTime.Now:yyyy_MM_dd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting representatives");
            TempData["Error"] = "خطا در خروجی گزارش";
            return RedirectToAction("RepresentativeReport");
        }
    }
}
