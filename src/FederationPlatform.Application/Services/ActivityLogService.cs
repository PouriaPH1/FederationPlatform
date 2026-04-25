using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivityLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task LogAsync(CreateActivityLogDto dto)
    {
        var log = new ActivityLog
        {
            UserId = dto.UserId,
            Action = dto.Action,
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            Details = dto.Details,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ActivityLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<ActivityLogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = 50)
    {
        var logs = await _unitOfWork.ActivityLogs.GetLogsAsync(pageNumber, pageSize);
        return _mapper.Map<List<ActivityLogDto>>(logs);
    }

    public async Task<List<ActivityLogDto>> GetUserLogsAsync(int userId, int pageNumber = 1, int pageSize = 50)
    {
        var logs = await _unitOfWork.ActivityLogs.GetUserLogsAsync(userId, pageNumber, pageSize);
        return _mapper.Map<List<ActivityLogDto>>(logs);
    }

    public async Task<List<ActivityLogDto>> GetEntityLogsAsync(string entityType, int entityId)
    {
        var logs = await _unitOfWork.ActivityLogs.GetEntityLogsAsync(entityType, entityId);
        return _mapper.Map<List<ActivityLogDto>>(logs);
    }

    public async Task<List<ActivityLogDto>> GetLogsByActionAsync(string action, int pageNumber = 1, int pageSize = 50)
    {
        var logs = await _unitOfWork.ActivityLogs.GetLogsByActionAsync(action, pageNumber, pageSize);
        return _mapper.Map<List<ActivityLogDto>>(logs);
    }

    public async Task<int> GetTotalLogsCountAsync()
    {
        return await _unitOfWork.ActivityLogs.GetTotalCountAsync();
    }

    public async Task DeleteOldLogsAsync(int daysToKeep = 90)
    {
        await _unitOfWork.ActivityLogs.DeleteOldLogsAsync(daysToKeep);
        await _unitOfWork.SaveChangesAsync();
    }
}
