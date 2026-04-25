using FederationPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Infrastructure.Services;

public interface IFileService : IDisposable
{
    Task<(bool Success, string FilePath, string? ErrorMessage)> UploadFileAsync(IFormFile file, string uploadFolder);
    Task<(bool Success, string? ErrorMessage)> DeleteFileAsync(string filePath);
    Task<(bool Success, byte[]? FileContent, string? ErrorMessage)> GetFileAsync(string filePath);
    Task<(bool Success, string FilePath, string? ErrorMessage)> UploadProfileImageAsync(IFormFile file, int userId);
    Task<(bool Success, string FilePath, string? ErrorMessage)> UploadActivityFileAsync(IFormFile file, int activityId);
    bool FileExists(string filePath);
    string GetFileExtension(string fileName);
}
