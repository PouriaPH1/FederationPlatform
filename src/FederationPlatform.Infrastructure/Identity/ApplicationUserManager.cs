using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FederationPlatform.Infrastructure.Identity;

public class ApplicationUserManager : UserManager<User>
{
    public ApplicationUserManager(IUserStore<User> store) : base(store, null, new PasswordHasher<User>(), null, null, null, null, null, null)
    {
    }

    public async Task<IdentityResult> CreateAdminAsync(string username, string email, string password)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            EmailConfirmed = true,
            Role = UserRole.Admin,
            IsActive = true
        };

        var result = await CreateAsync(user, password);
        return result;
    }

    public async Task<IdentityResult> CreateRepresentativeAsync(string username, string email, string password)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            EmailConfirmed = false,
            Role = UserRole.Representative,
            IsActive = true
        };

        var result = await CreateAsync(user, password);
        return result;
    }

    public async Task<IdentityResult> PromoteToRepresentativeAsync(User user)
    {
        if (user.Role == UserRole.Representative)
            return IdentityResult.Failed(new IdentityError { Description = "User is already a representative." });

        user.Role = UserRole.Representative;
        return await UpdateAsync(user);
    }

    public async Task<IdentityResult> BanUserAsync(User user)
    {
        user.IsActive = false;
        return await UpdateAsync(user);
    }

    public async Task<IdentityResult> UnbanUserAsync(User user)
    {
        user.IsActive = true;
        return await UpdateAsync(user);
    }
}
