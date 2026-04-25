using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace FederationPlatform.Application.Services;

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public ActivityService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<ActivityDto?> GetActivityByIdAsync(int id)
    {
        var activity = await _unitOfWork.Activities.GetByIdWithDetailsAsync(id);
        return activity == null ? null : _mapper.Map<ActivityDto>(activity);
    }

    public async Task<IEnumerable<ActivityListDto>> GetAllActivitiesAsync()
    {
        var activities = await _unitOfWork.Activities.GetAllAsync();
        return _mapper.Map<IEnumerable<ActivityListDto>>(activities);
    }

    public async Task<IEnumerable<ActivityListDto>> GetApprovedActivitiesAsync()
    {
        var activities = await _unitOfWork.Activities.GetApprovedAsync();
        return _mapper.Map<IEnumerable<ActivityListDto>>(activities);
    }

    public async Task<IEnumerable<ActivityListDto>> GetPendingActivitiesAsync()
    {
        var activities = await _unitOfWork.Activities.GetPendingAsync();
        return _mapper.Map<IEnumerable<ActivityListDto>>(activities);
    }

    public async Task<IEnumerable<ActivityListDto>> GetByUserIdAsync(int userId)
    {
        var activities = await _unitOfWork.Activities.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ActivityListDto>>(activities);
    }

    public async Task<IEnumerable<ActivityListDto>> GetByUniversityIdAsync(int universityId)
    {
        var activities = await _unitOfWork.Activities.GetByUniversityIdAsync(universityId);
        return _mapper.Map<IEnumerable<ActivityListDto>>(activities);
    }

    public async Task<PagedResultDto<ActivityListDto>> SearchActivitiesAsync(ActivityFilterDto filter)
    {
        var all = await _unitOfWork.Activities.SearchAsync(
            filter.SearchTerm, filter.UniversityId, filter.OrganizationId, filter.ActivityType, filter.Status);

        var list = all.ToList();
        var totalCount = list.Count;
        var items = list
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return new PagedResultDto<ActivityListDto>
        {
            Items = _mapper.Map<IList<ActivityListDto>>(items),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ActivityDto> CreateActivityAsync(int userId, CreateActivityDto dto)
    {
        var activity = _mapper.Map<Activity>(dto);
        activity.UserId = userId;
        activity.Status = ActivityStatus.Pending;

        await _unitOfWork.Activities.AddAsync(activity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ActivityDto>(activity);
    }

    public async Task<bool> UpdateActivityAsync(int id, int userId, UpdateActivityDto dto)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(id);
        if (activity == null) return false;
        if (activity.UserId != userId && activity.Status != ActivityStatus.Pending) return false;

        _mapper.Map(dto, activity);
        await _unitOfWork.Activities.UpdateAsync(activity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteActivityAsync(int id, int userId)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(id);
        if (activity == null) return false;
        if (activity.UserId != userId) return false;

        await _unitOfWork.Activities.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveActivityAsync(int id)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(id);
        if (activity == null) return false;

        activity.Status = ActivityStatus.Approved;
        await _unitOfWork.Activities.UpdateAsync(activity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectActivityAsync(int id)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(id);
        if (activity == null) return false;

        activity.Status = ActivityStatus.Rejected;
        await _unitOfWork.Activities.UpdateAsync(activity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UploadActivityFileAsync(int activityId, IFormFile file)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(activityId);
        if (activity == null) return false;

        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType)) return false;
        if (file.Length > 10 * 1024 * 1024) return false;

        var path = await _fileService.UploadFileAsync(file, "activities");
        if (path == null) return false;

        var activityFile = new ActivityFile
        {
            ActivityId = activityId,
            FileName = file.FileName,
            FilePath = path,
            FileType = file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        await _unitOfWork.ActivityFiles.AddAsync(activityFile);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteActivityFileAsync(int fileId)
    {
        var file = await _unitOfWork.ActivityFiles.GetByIdAsync(fileId);
        if (file == null) return false;

        await _fileService.DeleteFileAsync(file.FilePath);
        await _unitOfWork.ActivityFiles.DeleteAsync(fileId);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
