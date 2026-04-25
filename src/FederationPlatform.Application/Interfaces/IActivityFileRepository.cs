using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IActivityFileRepository
{
    Task<ActivityFile?> GetByIdAsync(int id);
    Task<IEnumerable<ActivityFile>> GetByActivityIdAsync(int activityId);
    Task<ActivityFile> AddAsync(ActivityFile file);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
