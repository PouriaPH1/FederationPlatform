using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IOrganizationService
{
    Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync();
    Task<OrganizationDto?> GetOrganizationByIdAsync(int id);
}
