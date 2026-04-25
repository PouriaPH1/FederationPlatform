using FederationPlatform.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Application.Services;

public interface IActivityService
{
    Task<ActivityDto?> GetActivityByIdAsync(int id);
    Task<IEnumerable<ActivityListDto>> GetAllActivitiesAsync();
    Task<IEnumerable<ActivityListDto>> GetApprovedActivitiesAsync();
    Task<IEnumerable<ActivityListDto>> GetPendingActivitiesAsync();
    Task<IEnumerable<ActivityListDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<ActivityListDto>> GetUserActivitiesAsync(int userId);
    Task<IEnumerable<ActivityListDto>> GetByUniversityIdAsync(int universityId);
    Task<IEnumerable<ActivityListDto>> GetUniversityActivitiesAsync(int universityId);
    Task<PagedResultDto<ActivityListDto>> SearchActivitiesAsync(ActivityFilterDto filter);
    Task<ActivityDto> CreateActivityAsync(int userId, CreateActivityDto dto);
    Task<bool> UpdateActivityAsync(int id, int userId, UpdateActivityDto dto);
    Task<bool> DeleteActivityAsync(int id, int userId);
    Task<bool> ApproveActivityAsync(int id);
    Task<bool> RejectActivityAsync(int id);
    Task<bool> UploadActivityFileAsync(int activityId, IFormFile file);
    Task<bool> DeleteActivityFileAsync(int fileId);
    Task<int> GetTotalActivitiesCountAsync();
    Task<IEnumerable<ActivityStatisticDto>> GetActivityStatisticsAsync();
}
