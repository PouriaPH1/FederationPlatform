using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Web.Models;
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
            
            if (!result.Succeeded)
            {
                model.ErrorMessage = "نام کاربری یا رمز عبور نادرست است";
                return View(model);
            }

            var user = result.Data;
            
            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add roles as claims
            if (user.Roles?.Any() ?? false)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
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
            var registerDto = new RegisterRequest
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _identityService.RegisterAsync(registerDto);
            
            if (!result.Succeeded)
            {
                model.ErrorMessage = result.Messages?.FirstOrDefault() ?? "خطایی در ثبت‌نام رخ داد";
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
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var userProfile = await _identityService.GetUserProfileAsync(userId);
            if (userProfile == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                Username = userProfile.UserName,
                Email = userProfile.Email,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Phone = userProfile.Phone,
                Address = userProfile.Address,
                City = userProfile.City,
                PostalCode = userProfile.PostalCode,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User",
                UniversityId = userProfile.UniversityId
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
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var updateDto = new UpdateProfileRequest
            {
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                UniversityId = model.UniversityId
            };

            var result = await _identityService.UpdateProfileAsync(updateDto);
            
            if (!result.Succeeded)
                return BadRequest(result.Messages?.FirstOrDefault() ?? "خطا در به‌روزرسانی پروفایل");

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
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var result = await _identityService.PromoteToRepresentativeAsync(userId);
            
            if (!result.Succeeded)
                return BadRequest(result.Messages?.FirstOrDefault() ?? "خطا در ترقی");

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
