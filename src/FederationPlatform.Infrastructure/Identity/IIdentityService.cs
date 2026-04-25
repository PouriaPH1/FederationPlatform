using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Infrastructure.Identity;

public interface IIdentityService
{
    Task<(bool Success, string Message)> RegisterAsync(string username, string email, string password);
    Task<(bool Success, User? User, string Message)> LoginAsync(string username, string password);
    Task<bool> ValidatePasswordAsync(User user, string password);
    Task<bool> PromoteToRepresentativeAsync(int userId);
    Task<bool> BanUserAsync(int userId);
}
