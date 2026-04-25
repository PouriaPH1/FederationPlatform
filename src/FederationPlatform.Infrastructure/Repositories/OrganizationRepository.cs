using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Organization>> GetActiveAsync()
    {
        return await _dbSet.Where(o => o.IsActive).ToListAsync();
    }
}
