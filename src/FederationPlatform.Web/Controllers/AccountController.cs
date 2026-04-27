using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Services;
using FederationPlatform.Infrastructure.Identity;
using FederationPlatform.Web.Models;
using FederationPlatform.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FederationPlatform.Web.Controllers;

public class AccountController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IIdentityService identityService, ILogger<AccountController> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _identityService.LoginAsync(model.Username, model.Password);
            
            if (!result.Success || result.User == null)
            {
                model.ErrorMessage = "نام کاربری یا رمز عبور نادرست است";
                return View(model);
            }

            var user = result.User;
            
            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User {Username} logged in successfully", model.Username);
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            model.ErrorMessage = "خطایی در حین ورود رخ داد";
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _identityService.RegisterAsync(model.Username, model.Email, model.Password);
            
            if (!result.Success)
            {
                model.ErrorMessage = result.Message ?? "خطایی در ثبت‌نام رخ داد";
                return View(model);
            }

            _logger.LogInformation("User {Username} registered successfully", model.Username);
            
            // Automatically log in after registration
            return RedirectToAction("Login", new { message = "ثبت‌نام موفق! لطفاً وارد شوید." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            model.ErrorMessage = "خطایی در حین ثبت‌نام رخ داد";
            return View(model);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
                return RedirectToAction("Login");

            var userProfile = await _identityService.GetUserProfileAsync(userIdInt);
            if (userProfile == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                Username = userProfile.Username,
                Email = userProfile.Email,
                FirstName = userProfile.UserProfile?.FirstName ?? "",
                LastName = userProfile.UserProfile?.LastName ?? "",
                Phone = userProfile.UserProfile?.PhoneNumber ?? "",
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User",
                UniversityId = userProfile.UserProfile?.UniversityId ?? 0
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile");
            return BadRequest("خطایی در بازیابی پروفایل رخ داد");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Profile", model);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
                return RedirectToAction("Login");

            var (success, message) = await _identityService.UpdateProfileAsync(
                userIdInt,
                model.FirstName,
                model.LastName,
                model.Phone,
                model.Address,
                model.City,
                model.PostalCode);
            
            if (!success)
                return BadRequest(message ?? "خطا در به‌روزرسانی پروفایل");

            return RedirectToAction("Profile", new { message = "پروفایل با موفقیت به‌روزرسانی شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return BadRequest("خطایی در حین به‌روزرسانی رخ داد");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return BadRequest("خطایی در حین خروج رخ داد");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpgradeToRepresentative()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
                return RedirectToAction("Login");

            var result = await _identityService.PromoteToRepresentativeAsync(userIdInt);
            
            if (!result)
                return BadRequest("خطا در ترقی");

            // Update claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, User.FindFirst(ClaimTypes.Name)?.Value ?? ""),
                new Claim(ClaimTypes.Email, User.FindFirst(ClaimTypes.Email)?.Value ?? ""),
                new Claim(ClaimTypes.Role, "Representative")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToAction("Profile", new { message = "ترقی موفق!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error promoting to representative");
            return BadRequest("خطایی در حین ترقی رخ داد");
        }
    }
}
