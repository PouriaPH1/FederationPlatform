using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Application.Mappings;

public class ActivityMappingProfile : Profile
{
    public ActivityMappingProfile()
    {
        CreateMap<Activity, ActivityDto>()
            .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src =>
                src.University != null ? src.University.Name : string.Empty))
            .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src =>
                src.Organization != null ? src.Organization.Name : string.Empty))
            .ForMember(dest => dest.SubmittedBy, opt => opt.MapFrom(src =>
                src.User != null ? src.User.Username : string.Empty))
            .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src =>
                src.ActivityType.ToString()))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src =>
                src.Status.ToString()));

        CreateMap<Activity, ActivityListDto>()
            .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src =>
                src.University != null ? src.University.Name : string.Empty))
            .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src =>
                src.Organization != null ? src.Organization.Name : string.Empty))
            .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src =>
                src.ActivityType.ToString()))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src =>
                src.Status.ToString()));

        CreateMap<ActivityFile, ActivityFileDto>();

        CreateMap<CreateActivityDto, Activity>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ActivityStatus.Pending))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.University, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityFiles, opt => opt.Ignore());

        CreateMap<UpdateActivityDto, Activity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.University, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityFiles, opt => opt.Ignore());
    }
}
