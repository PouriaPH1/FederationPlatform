using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;

namespace FederationPlatform.Application.Services;

public class UniversityService : IUniversityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UniversityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UniversityDto>> GetAllUniversitiesAsync()
    {
        var universities = await _unitOfWork.Universities.GetActiveAsync();
        return _mapper.Map<IEnumerable<UniversityDto>>(universities);
    }

    public async Task<UniversityDetailDto?> GetUniversityByIdAsync(int id)
    {
        var university = await _unitOfWork.Universities.GetByIdAsync(id);
        if (university == null) return null;

        var dto = _mapper.Map<UniversityDetailDto>(university);
        var activities = await _unitOfWork.Activities.GetByUniversityIdAsync(id);
        var activityList = activities.ToList();

        dto.ActivityCount = activityList.Count;
        dto.RecentActivities = _mapper.Map<IList<ActivityListDto>>(
            activityList.OrderByDescending(a => a.CreatedAt).Take(5).ToList());

        return dto;
    }

    public async Task<IEnumerable<ActivityListDto>> GetUniversityActivitiesAsync(int universityId)
    {
        var activities = await _unitOfWork.Activities.GetByUniversityIdAsync(universityId);
        return _mapper.Map<IEnumerable<ActivityListDto>>(activities);
    }
}
