using FederationPlatform.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FederationPlatform.Web.Controllers;

public class NewsController : Controller
{
    private readonly INewsService _newsService;
    private readonly ILogger<NewsController> _logger;

    public NewsController(INewsService newsService, ILogger<NewsController> logger)
    {
        _newsService = newsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        try
        {
            var allNews = await _newsService.GetAllNewsAsync();
            var totalCount = allNews.Count();
            var news = allNews
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(news);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news");
            TempData["Error"] = "خطا در بارگیری اخبار";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound();

            return View(news);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news details");
            TempData["Error"] = "خطا در بارگیری جزئیات خبر";
            return RedirectToAction("Index");
        }
    }
}
