using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class UniversityRepository : RepositoryBase<University>, IUniversityRepository
{
    public UniversityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<University>> GetActiveAsync()
    {
        return await _dbSet.Where(u => u.IsActive).ToListAsync();
    }
}
