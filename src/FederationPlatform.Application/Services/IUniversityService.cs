using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IUniversityService
{
    Task<IEnumerable<UniversityDto>> GetAllUniversitiesAsync();
    Task<UniversityDetailDto?> GetUniversityByIdAsync(int id);
    Task<IEnumerable<ActivityListDto>> GetUniversityActivitiesAsync(int universityId);
}
