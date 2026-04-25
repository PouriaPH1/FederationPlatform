using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class ActivityFileRepository : RepositoryBase<ActivityFile>, IActivityFileRepository
{
    public ActivityFileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ActivityFile>> GetByActivityIdAsync(int activityId)
    {
        return await _dbSet.Where(af => af.ActivityId == activityId).ToListAsync();
    }
}
