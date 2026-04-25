using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(int id);
    Task<IEnumerable<Organization>> GetAllAsync();
    Task<IEnumerable<Organization>> GetActiveAsync();
    Task<Organization> AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
