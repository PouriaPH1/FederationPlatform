using BCrypt.Net;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;
using BC = BCrypt.Net.BCrypt;

namespace FederationPlatform.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWork _unitOfWork;

    public IdentityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string username, string email, string password)
    {
        try
        {
            // Check if username exists
            if (await _unitOfWork.Users.UsernameExistsAsync(username))
                return (false, "نام کاربری قبلاً ثبت شده است.");

            // Check if email exists
            if (await _unitOfWork.Users.EmailExistsAsync(email))
                return (false, "ایمیل قبلاً ثبت شده است.");

            // Validate password
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return (false, "رمز عبور باید حداقل 6 کاراکتر باشد.");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BC.HashPassword(password),
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "ثبت نام موفق بود.");
        }
        catch (Exception ex)
        {
            return (false, $"خطا در ثبت نام: {ex.Message}");
        }
    }

    public async Task<(bool Success, User? User, string Message)> LoginAsync(string username, string password)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);

            if (user == null)
                return (false, null, "نام کاربری یا رمز عبور نادرست است.");

            if (!user.IsActive)
                return (false, null, "حساب کاربری شما غیر فعال است.");

            if (!BC.Verify(password, user.PasswordHash))
                return (false, null, "نام کاربری یا رمز عبور نادرست است.");

            return (true, user, "ورود موفق بود.");
        }
        catch (Exception ex)
        {
            return (false, null, $"خطا در ورود: {ex.Message}");
        }
    }

    public async Task<bool> ValidatePasswordAsync(User user, string password)
    {
        return await Task.FromResult(BC.Verify(password, user.PasswordHash));
    }

    public async Task<bool> PromoteToRepresentativeAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Role = UserRole.Representative;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> BanUserAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetUserProfileAsync(int userId)
    {
        try
        {
            return await _unitOfWork.Users.GetByIdAsync(userId);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, string firstName, string lastName, string? phone, string? address, string? city, string? postalCode)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return (false, "کاربر یافت نشد.");

            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "پروفایل با موفقیت به‌روزرسانی شد.");
        }
        catch (Exception ex)
        {
            return (false, $"خطا در به‌روزرسانی پروفایل: {ex.Message}");
        }
    }
}
