using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _dbSet
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetUnreadNotificationsAsync(int userId)
    {
        return await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(int id)
    {
        var notification = await GetByIdAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _dbSet.Update(notification);
        }

        await Task.CompletedTask;
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await Task.CompletedTask;
    }

    public async Task DeleteOldNotificationsAsync(int userId, int daysToKeep = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        var oldNotifications = await _dbSet
            .Where(n => n.UserId == userId && n.CreatedAt < cutoffDate && n.IsRead)
            .ToListAsync();

        foreach (var notification in oldNotifications)
        {
            _dbSet.Remove(notification);
        }

        await Task.CompletedTask;
    }
}
