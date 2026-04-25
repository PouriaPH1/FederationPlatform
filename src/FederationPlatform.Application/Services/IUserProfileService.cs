using FederationPlatform.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Application.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetProfileByUserIdAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, UpdateUserProfileDto dto);
    Task<string?> UploadProfileImageAsync(int userId, IFormFile file);
    Task<string?> UploadResumeAsync(int userId, IFormFile file);
}
