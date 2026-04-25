using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Infrastructure.Repositories;

public class WorkshopRepository : RepositoryBase<Workshop>, IWorkshopRepository
{
    public WorkshopRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Workshop>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(w => w.StartDate >= now)
            .OrderBy(w => w.StartDate)
            .Include(w => w.CreatedByUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workshop>> GetUpcomingAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(w => w.StartDate >= now)
            .OrderBy(w => w.StartDate)
            .Include(w => w.CreatedByUser)
            .ToListAsync();
    }
}
