using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
{
    public ActivityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Activity?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(a => a.University)
            .Include(a => a.Organization)
            .Include(a => a.User)
            .Include(a => a.ActivityFiles)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Activity>> GetApprovedAsync()
    {
        return await _dbSet
            .Where(a => a.Status == ActivityStatus.Approved)
            .Include(a => a.University)
            .Include(a => a.Organization)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetPendingAsync()
    {
        return await _dbSet
            .Where(a => a.Status == ActivityStatus.Pending)
            .Include(a => a.University)
            .Include(a => a.Organization)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .Include(a => a.University)
            .Include(a => a.Organization)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetByUniversityIdAsync(int universityId)
    {
        return await _dbSet
            .Where(a => a.UniversityId == universityId)
            .Include(a => a.Organization)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetByOrganizationIdAsync(int organizationId)
    {
        return await _dbSet
            .Where(a => a.OrganizationId == organizationId)
            .Include(a => a.University)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetByStatusAsync(ActivityStatus status)
    {
        return await _dbSet
            .Where(a => a.Status == status)
            .Include(a => a.University)
            .Include(a => a.Organization)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> SearchAsync(string? searchTerm, int? universityId, int? organizationId, ActivityType? type, ActivityStatus? status)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Title.Contains(searchTerm) || a.Description.Contains(searchTerm));
        }

        if (universityId.HasValue)
        {
            query = query.Where(a => a.UniversityId == universityId.Value);
        }

        if (organizationId.HasValue)
        {
            query = query.Where(a => a.OrganizationId == organizationId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(a => a.ActivityType == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query
            .Include(a => a.University)
            .Include(a => a.Organization)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<int> CountByStatusAsync(ActivityStatus status)
    {
        return await _dbSet.CountAsync(a => a.Status == status);
    }

    public async Task<int> CountByUniversityAsync(int universityId)
    {
        return await _dbSet.CountAsync(a => a.UniversityId == universityId && a.Status == ActivityStatus.Approved);
    }
}
