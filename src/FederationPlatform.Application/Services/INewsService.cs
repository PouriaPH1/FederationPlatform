using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface INewsService
{
    Task<IEnumerable<NewsDto>> GetAllNewsAsync();
    Task<IEnumerable<NewsDto>> GetPublishedNewsAsync();
    Task<IEnumerable<NewsDto>> GetLatestNewsAsync(int count);
    Task<NewsDto?> GetNewsByIdAsync(int id);
    Task<NewsDto> CreateNewsAsync(int adminUserId, CreateNewsDto dto);
    Task<bool> UpdateNewsAsync(int id, UpdateNewsDto dto);
    Task<bool> DeleteNewsAsync(int id);
    Task<bool> PublishNewsAsync(int id);
}
