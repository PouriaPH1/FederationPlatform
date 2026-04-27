using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FederationPlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResultDto> RegisterAsync(CreateUserDto dto)
    {
        if (await _unitOfWork.Users.UsernameExistsAsync(dto.Username))
            return new AuthResultDto { Succeeded = false, ErrorMessage = "این نام کاربری قبلاً استفاده شده است." };

        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            return new AuthResultDto { Succeeded = false, ErrorMessage = "این ایمیل قبلاً ثبت شده است." };

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = UserRole.User,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);

        var profile = new UserProfile
        {
            UserId = user.Id,
            PhoneNumber = dto.PhoneNumber ?? string.Empty,
            UniversityId = dto.UniversityId,
            Major = dto.FieldOfStudy
        };
        await _unitOfWork.UserProfiles.AddAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResultDto
        {
            Succeeded = true,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            return new AuthResultDto { Succeeded = false, ErrorMessage = "نام کاربری یا رمز عبور اشتباه است." };

        if (!user.IsActive)
            return new AuthResultDto { Succeeded = false, ErrorMessage = "حساب کاربری شما غیرفعال شده است." };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        return new AuthResultDto
        {
            Succeeded = true,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task LogoutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return false;

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null || !VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(dto.NewPassword);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
