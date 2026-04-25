using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IActivityLogService
{
    Task LogAsync(CreateActivityLogDto dto);
    Task<List<ActivityLogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = 50);
    Task<List<ActivityLogDto>> GetUserLogsAsync(int userId, int pageNumber = 1, int pageSize = 50);
    Task<List<ActivityLogDto>> GetEntityLogsAsync(string entityType, int entityId);
    Task<List<ActivityLogDto>> GetLogsByActionAsync(string action, int pageNumber = 1, int pageSize = 50);
    Task<int> GetTotalLogsCountAsync();
    Task DeleteOldLogsAsync(int daysToKeep = 90);
}
