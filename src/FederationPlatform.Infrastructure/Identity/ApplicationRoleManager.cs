using FederationPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FederationPlatform.Infrastructure.Identity;

public class ApplicationRoleManager : RoleManager<IdentityRole>
{
    public ApplicationRoleManager(IRoleStore<IdentityRole> store) : base(store, null, null, null, null)
    {
    }

    public async Task<bool> InitializeRolesAsync()
    {
        var roles = new[] { "Admin", "Representative", "User" };
        
        foreach (var role in roles)
        {
            if (!await RoleExistsAsync(role))
            {
                var result = await CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                    return false;
            }
        }

        return true;
    }
}
