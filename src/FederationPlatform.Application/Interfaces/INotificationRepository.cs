using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface INotificationRepository
{
    Task<Notification> AddAsync(Notification notification);
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetUserNotificationsAsync(int userId);
    Task<List<Notification>> GetUnreadNotificationsAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int id);
    Task MarkAllAsReadAsync(int userId);
    Task DeleteAsync(int id);
    Task DeleteOldNotificationsAsync(int userId, int daysToKeep = 30);
}
