using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;

namespace FederationPlatform.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrganizationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync()
    {
        var orgs = await _unitOfWork.Organizations.GetActiveAsync();
        return _mapper.Map<IEnumerable<OrganizationDto>>(orgs);
    }

    public async Task<OrganizationDto?> GetOrganizationByIdAsync(int id)
    {
        var org = await _unitOfWork.Organizations.GetByIdAsync(id);
        if (org == null) return null;

        var dto = _mapper.Map<OrganizationDto>(org);
        var activities = await _unitOfWork.Activities.GetByOrganizationIdAsync(id);
        dto.ActivityCount = activities.Count();
        return dto;
    }
}
