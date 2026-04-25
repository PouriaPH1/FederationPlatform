using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IWorkshopRepository
{
    Task<Workshop?> GetByIdAsync(int id);
    Task<IEnumerable<Workshop>> GetAllAsync();
    Task<IEnumerable<Workshop>> GetActiveAsync();
    Task<IEnumerable<Workshop>> GetUpcomingAsync();
    Task<Workshop> AddAsync(Workshop workshop);
    Task UpdateAsync(Workshop workshop);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
