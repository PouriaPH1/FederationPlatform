using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IUniversityRepository
{
    Task<University?> GetByIdAsync(int id);
    Task<IEnumerable<University>> GetAllAsync();
    Task<IEnumerable<University>> GetActiveAsync();
    Task<University> AddAsync(University university);
    Task UpdateAsync(University university);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
