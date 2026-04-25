using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Services;

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NewsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NewsDto>> GetAllNewsAsync()
    {
        var news = await _unitOfWork.News.GetAllAsync();
        return _mapper.Map<IEnumerable<NewsDto>>(news);
    }

    public async Task<IEnumerable<NewsDto>> GetPublishedNewsAsync()
    {
        var news = await _unitOfWork.News.GetPublishedAsync();
        return _mapper.Map<IEnumerable<NewsDto>>(news);
    }

    public async Task<IEnumerable<NewsDto>> GetLatestNewsAsync(int count)
    {
        var news = await _unitOfWork.News.GetLatestAsync(count);
        return _mapper.Map<IEnumerable<NewsDto>>(news);
    }

    public async Task<NewsDto?> GetNewsByIdAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        return news == null ? null : _mapper.Map<NewsDto>(news);
    }

    public async Task<NewsDto> CreateNewsAsync(int adminUserId, CreateNewsDto dto)
    {
        var news = _mapper.Map<News>(dto);
        news.CreatedBy = adminUserId;
        news.PublishedAt = dto.IsPublished ? DateTime.UtcNow : DateTime.MinValue;

        await _unitOfWork.News.AddAsync(news);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<NewsDto>(news);
    }

    public async Task<bool> UpdateNewsAsync(int id, UpdateNewsDto dto)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null) return false;

        news.Title = dto.Title;
        news.Content = dto.Content;
        news.ImageUrl = dto.ImageUrl;
        if (dto.IsPublished && !news.IsPublished)
            news.PublishedAt = DateTime.UtcNow;
        news.IsPublished = dto.IsPublished;

        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNewsAsync(int id)
    {
        if (!await _unitOfWork.News.ExistsAsync(id)) return false;
        await _unitOfWork.News.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishNewsAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null) return false;

        news.IsPublished = true;
        news.PublishedAt = DateTime.UtcNow;
        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
