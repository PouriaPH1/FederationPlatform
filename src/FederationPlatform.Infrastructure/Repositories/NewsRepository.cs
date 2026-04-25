using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Infrastructure.Repositories;

public class NewsRepository : RepositoryBase<News>, INewsRepository
{
    public NewsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<News>> GetPublishedAsync()
    {
        return await _dbSet
            .Where(n => n.PublishedAt <= DateTime.UtcNow)
            .OrderByDescending(n => n.PublishedAt)
            .Include(n => n.CreatedByUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetLatestAsync(int count)
    {
        return await _dbSet
            .OrderByDescending(n => n.PublishedAt)
            .Take(count)
            .Include(n => n.CreatedByUser)
            .ToListAsync();
    }
}
