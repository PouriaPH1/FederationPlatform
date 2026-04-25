using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                src.UserProfile != null
                    ? $"{src.UserProfile.FirstName} {src.UserProfile.LastName}".Trim()
                    : null))
            .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src =>
                src.UserProfile != null && src.UserProfile.University != null
                    ? src.UserProfile.University.Name
                    : null));

        CreateMap<UserProfile, UserProfileDto>()
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src =>
                src.University != null ? src.University.Name : null));

        CreateMap<UpdateUserProfileDto, UserProfile>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
