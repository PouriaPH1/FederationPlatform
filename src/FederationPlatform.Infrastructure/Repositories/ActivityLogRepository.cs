using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class ActivityLogRepository : RepositoryBase<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<ActivityLog>> GetLogsAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .Include(l => l.User)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<ActivityLog>> GetUserLogsAsync(int userId, int pageNumber, int pageSize)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<ActivityLog>> GetEntityLogsAsync(string entityType, int entityId)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.EntityType == entityType && l.EntityId == entityId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ActivityLog>> GetLogsByActionAsync(string action, int pageNumber, int pageSize)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.Action == action)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task DeleteOldLogsAsync(int daysToKeep)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        var oldLogs = await _dbSet
            .Where(l => l.CreatedAt < cutoffDate)
            .ToListAsync();

        _dbSet.RemoveRange(oldLogs);
    }
}
