using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
    Task<NotificationListDto?> GetNotificationByIdAsync(int id);
    Task<List<NotificationListDto>> GetUserNotificationsAsync(int userId);
    Task<List<NotificationListDto>> GetUnreadNotificationsAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task<bool> MarkAsReadAsync(int id);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<bool> DeleteNotificationAsync(int id);
    Task SendActivityApprovalNotificationAsync(int userId, int activityId, string activityTitle);
    Task SendActivityRejectionNotificationAsync(int userId, int activityId, string activityTitle);
    Task SendNewActivityNotificationToAdminAsync(int activityId, string activityTitle, string representativeName);
    Task SendUserBannedNotificationAsync(int userId, string reason);
    Task SendUserActivatedNotificationAsync(int userId, string message);
    Task SendUserPromotedNotificationAsync(int userId, string title, string message);
}
