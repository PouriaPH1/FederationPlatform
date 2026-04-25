using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = _mapper.Map<Notification>(dto);
        notification.CreatedAt = DateTime.UtcNow;
        notification.IsRead = false;

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task<NotificationListDto?> GetNotificationByIdAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
        return notification == null ? null : _mapper.Map<NotificationListDto>(notification);
    }

    public async Task<List<NotificationListDto>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _unitOfWork.Notifications.GetUserNotificationsAsync(userId);
        return _mapper.Map<List<NotificationListDto>>(notifications);
    }

    public async Task<List<NotificationListDto>> GetUnreadNotificationsAsync(int userId)
    {
        var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsAsync(userId);
        return _mapper.Map<List<NotificationListDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _unitOfWork.Notifications.GetUnreadCountAsync(userId);
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsAsync(userId);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        if (notifications.Count > 0)
            await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
        if (notification == null)
            return false;

        await _unitOfWork.Notifications.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task SendActivityApprovalNotificationAsync(int userId, int activityId, string activityTitle)
    {
        var notification = new CreateNotificationDto
        {
            UserId = userId,
            Title = "✅ فعالیت تایید شد",
            Message = $"فعالیت \"{activityTitle}\" توسط مدیر تایید شد.",
            Type = "Activity",
            ActivityId = activityId
        };

        await CreateNotificationAsync(notification);
    }

    public async Task SendActivityRejectionNotificationAsync(int userId, int activityId, string activityTitle)
    {
        var notification = new CreateNotificationDto
        {
            UserId = userId,
            Title = "❌ فعالیت رد شد",
            Message = $"فعالیت \"{activityTitle}\" توسط مدیر رد شد.",
            Type = "Activity",
            ActivityId = activityId
        };

        await CreateNotificationAsync(notification);
    }

    public async Task SendNewActivityNotificationToAdminAsync(int activityId, string activityTitle, string representativeName)
    {
        // Get all admins
        var admins = await _unitOfWork.Users.GetAdminsAsync();

        foreach (var admin in admins)
        {
            var notification = new CreateNotificationDto
            {
                UserId = admin.Id,
                Title = "📝 فعالیت جدید برای بررسی",
                Message = $"نماینده {representativeName} فعالیت جدید \"{activityTitle}\" ثبت کرد.",
                Type = "Activity",
                ActivityId = activityId
            };

            await CreateNotificationAsync(notification);
        }
    }
}
