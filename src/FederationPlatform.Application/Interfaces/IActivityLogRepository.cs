using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IActivityLogRepository
{
    Task<ActivityLog> AddAsync(ActivityLog log);
    Task<List<ActivityLog>> GetLogsAsync(int pageNumber, int pageSize);
    Task<List<ActivityLog>> GetUserLogsAsync(int userId, int pageNumber, int pageSize);
    Task<List<ActivityLog>> GetEntityLogsAsync(string entityType, int entityId);
    Task<List<ActivityLog>> GetLogsByActionAsync(string action, int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task DeleteOldLogsAsync(int daysToKeep);
}
