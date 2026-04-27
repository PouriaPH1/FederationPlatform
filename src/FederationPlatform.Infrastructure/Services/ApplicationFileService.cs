using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Infrastructure.Services;

public class ApplicationFileService : FederationPlatform.Application.Services.IFileService
{
    private readonly IFileService _fileService;

    public ApplicationFileService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<string?> UploadFileAsync(IFormFile file, string folder)
    {
        var result = await _fileService.UploadFileAsync(file, folder);
        return result.Success ? result.FilePath : null;
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        var result = await _fileService.DeleteFileAsync(filePath);
        return result.Success;
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        return filePath.StartsWith("/", StringComparison.Ordinal) ? filePath : $"/{filePath}";
    }
}
