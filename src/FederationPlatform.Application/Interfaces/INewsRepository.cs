using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface INewsRepository
{
    Task<News?> GetByIdAsync(int id);
    Task<IEnumerable<News>> GetAllAsync();
    Task<IEnumerable<News>> GetPublishedAsync();
    Task<IEnumerable<News>> GetLatestAsync(int count);
    Task<News> AddAsync(News news);
    Task UpdateAsync(News news);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
