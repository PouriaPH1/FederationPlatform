using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IWorkshopService
{
    Task<IEnumerable<WorkshopDto>> GetAllWorkshopsAsync();
    Task<IEnumerable<WorkshopDto>> GetActiveWorkshopsAsync();
    Task<IEnumerable<WorkshopDto>> GetUpcomingWorkshopsAsync();
    Task<WorkshopDto?> GetWorkshopByIdAsync(int id);
    Task<WorkshopDto> CreateWorkshopAsync(int adminUserId, CreateWorkshopDto dto);
    Task<bool> UpdateWorkshopAsync(int id, UpdateWorkshopDto dto);
    Task<bool> DeleteWorkshopAsync(int id);
}
