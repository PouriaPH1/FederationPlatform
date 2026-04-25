using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Mappings;

public class UniversityMappingProfile : Profile
{
    public UniversityMappingProfile()
    {
        CreateMap<University, UniversityDto>();

        CreateMap<University, UniversityDetailDto>()
            .ForMember(dest => dest.ActivityCount, opt => opt.Ignore())
            .ForMember(dest => dest.RepresentativeCount, opt => opt.Ignore())
            .ForMember(dest => dest.RecentActivities, opt => opt.Ignore());

        CreateMap<Organization, OrganizationDto>()
            .ForMember(dest => dest.ActivityCount, opt => opt.Ignore());
    }
}
