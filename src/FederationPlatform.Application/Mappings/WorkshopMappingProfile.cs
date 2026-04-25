using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Mappings;

public class WorkshopMappingProfile : Profile
{
    public WorkshopMappingProfile()
    {
        CreateMap<Workshop, WorkshopDto>()
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src =>
                src.Creator != null ? src.Creator.Username : string.Empty));

        CreateMap<CreateWorkshopDto, Workshop>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Creator, opt => opt.Ignore());

        CreateMap<UpdateWorkshopDto, Workshop>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Creator, opt => opt.Ignore());
    }
}
