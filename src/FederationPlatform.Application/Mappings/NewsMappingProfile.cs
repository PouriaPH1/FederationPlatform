using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Mappings;

public class NewsMappingProfile : Profile
{
    public NewsMappingProfile()
    {
        CreateMap<News, NewsDto>()
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src =>
                src.Creator != null ? src.Creator.Username : string.Empty));

        CreateMap<CreateNewsDto, News>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Creator, opt => opt.Ignore());

        CreateMap<UpdateNewsDto, News>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Creator, opt => opt.Ignore());
    }
}
