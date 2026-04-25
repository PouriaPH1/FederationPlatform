using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<UserProfileDto?> GetProfileByUserIdAsync(int userId)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null) return null;

        var dto = _mapper.Map<UserProfileDto>(profile);
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user != null) dto.Username = user.Username;

        if (profile.UniversityId.HasValue)
        {
            var university = await _unitOfWork.Universities.GetByIdAsync(profile.UniversityId.Value);
            if (university != null) dto.UniversityName = university.Name;
        }

        return dto;
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateUserProfileDto dto)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);

        if (profile == null)
        {
            profile = new UserProfile { UserId = userId };
            _mapper.Map(dto, profile);
            await _unitOfWork.UserProfiles.AddAsync(profile);
        }
        else
        {
            _mapper.Map(dto, profile);
            await _unitOfWork.UserProfiles.UpdateAsync(profile);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<string?> UploadProfileImageAsync(int userId, IFormFile file)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType)) return null;

        var path = await _fileService.UploadFileAsync(file, "profiles");
        if (path == null) return null;

        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null)
        {
            profile = new UserProfile { UserId = userId, ProfileImageUrl = path };
            await _unitOfWork.UserProfiles.AddAsync(profile);
        }
        else
        {
            profile.ProfileImageUrl = path;
            await _unitOfWork.UserProfiles.UpdateAsync(profile);
        }

        await _unitOfWork.SaveChangesAsync();
        return path;
    }

    public async Task<string?> UploadResumeAsync(int userId, IFormFile file)
    {
        if (file.ContentType != "application/pdf") return null;

        var path = await _fileService.UploadFileAsync(file, "resumes");
        if (path == null) return null;

        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null)
        {
            profile = new UserProfile { UserId = userId, ResumeUrl = path };
            await _unitOfWork.UserProfiles.AddAsync(profile);
        }
        else
        {
            profile.ResumeUrl = path;
            await _unitOfWork.UserProfiles.UpdateAsync(profile);
        }

        await _unitOfWork.SaveChangesAsync();
        return path;
    }
}
