using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(int userId);
    Task<UserProfile?> GetByIdAsync(int id);
    Task<UserProfile> AddAsync(UserProfile profile);
    Task UpdateAsync(UserProfile profile);
    Task<bool> ExistsForUserAsync(int userId);
}
