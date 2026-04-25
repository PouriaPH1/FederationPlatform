using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Application.Services;

public interface IFileService
{
    Task<string?> UploadFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
