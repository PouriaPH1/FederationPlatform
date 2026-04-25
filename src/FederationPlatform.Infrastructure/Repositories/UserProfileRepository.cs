using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Infrastructure.Repositories;

public class UserProfileRepository : RepositoryBase<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<bool> ExistsForUserAsync(int userId)
    {
        return await _dbSet.AnyAsync(up => up.UserId == userId);
    }
}
