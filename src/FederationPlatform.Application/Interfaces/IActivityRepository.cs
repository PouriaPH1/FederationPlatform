using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Application.Interfaces;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(int id);
    Task<Activity?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Activity>> GetAllAsync();
    Task<IEnumerable<Activity>> GetApprovedAsync();
    Task<IEnumerable<Activity>> GetPendingAsync();
    Task<IEnumerable<Activity>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Activity>> GetByUniversityIdAsync(int universityId);
    Task<IEnumerable<Activity>> GetByOrganizationIdAsync(int organizationId);
    Task<IEnumerable<Activity>> GetByStatusAsync(ActivityStatus status);
    Task<IEnumerable<Activity>> SearchAsync(string? searchTerm, int? universityId, int? organizationId, ActivityType? type, ActivityStatus? status);
    Task<Activity> AddAsync(Activity activity);
    Task UpdateAsync(Activity activity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task<int> CountByStatusAsync(ActivityStatus status);
    Task<int> CountByUniversityAsync(int universityId);
}
