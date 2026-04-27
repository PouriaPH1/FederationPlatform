using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _uploadsFolder;
    private readonly string _profileImagesFolder;
    private readonly string _activityFilesFolder;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif", ".xls", ".xlsx", ".zip" };

    public FileService(string uploadsRootPath = "uploads")
    {
        _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadsRootPath);
        _profileImagesFolder = Path.Combine(_uploadsFolder, "profile-images");
        _activityFilesFolder = Path.Combine(_uploadsFolder, "activity-files");

        CreateDirectoriesIfNotExist();
    }

    private void CreateDirectoriesIfNotExist()
    {
        Directory.CreateDirectory(_uploadsFolder);
        Directory.CreateDirectory(_profileImagesFolder);
        Directory.CreateDirectory(_activityFilesFolder);
    }

    public async Task<(bool Success, string FilePath, string? ErrorMessage)> UploadFileAsync(IFormFile file, string uploadFolder)
    {
        try
        {
            if (file == null || file.Length == 0)
                return (false, string.Empty, "فایل انتخاب نشده است.");

            if (file.Length > MaxFileSize)
                return (false, string.Empty, $"حجم فایل نمی‌تواند بیشتر از {MaxFileSize / (1024 * 1024)} مگابایت باشد.");

            var extension = GetFileExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension.ToLower()))
                return (false, string.Empty, "نوع فایل مجاز نیست.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine(_uploadsFolder, uploadFolder);
            Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", uploadFolder, fileName).Replace("\\", "/");
            return (true, relativePath, null);
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"خطا در آپلود فایل: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                return (true, null);
            }
            return (false, "فایل یافت نشد.");
        }
        catch (Exception ex)
        {
            return (false, $"خطا در حذف فایل: {ex.Message}");
        }
    }

    public async Task<(bool Success, byte[]? FileContent, string? ErrorMessage)> GetFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            if (!File.Exists(fullPath))
                return (false, null, "فایل یافت نشد.");

            var fileContent = await File.ReadAllBytesAsync(fullPath);
            return (true, fileContent, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"خطا در دانلود فایل: {ex.Message}");
        }
    }

    public async Task<(bool Success, string FilePath, string? ErrorMessage)> UploadProfileImageAsync(IFormFile file, int userId)
    {
        try
        {
            if (file == null || file.Length == 0)
                return (false, string.Empty, "تصویر انتخاب نشده است.");

            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = GetFileExtension(file.FileName).ToLower();
            
            if (!allowedImageExtensions.Contains(extension))
                return (false, string.Empty, "فقط فایل‌های تصویری مجاز هستند.");

            if (file.Length > 5 * 1024 * 1024) // 5 MB for images
                return (false, string.Empty, "حجم تصویر نمی‌تواند بیشتر از 5 مگابایت باشد.");

            var fileName = $"user-{userId}-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_profileImagesFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "profile-images", fileName).Replace("\\", "/");
            return (true, relativePath, null);
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"خطا در آپلود تصویر: {ex.Message}");
        }
    }

    public async Task<(bool Success, string FilePath, string? ErrorMessage)> UploadActivityFileAsync(IFormFile file, int activityId)
    {
        try
        {
            var result = await UploadFileAsync(file, "activity-files");
            if (!result.Success)
                return result;

            return result;
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"خطا در آپلود فایل فعالیت: {ex.Message}");
        }
    }

    public bool FileExists(string filePath)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
        return File.Exists(fullPath);
    }

    public string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
