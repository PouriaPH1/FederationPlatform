using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<CreateNotificationDto, Notification>();

        CreateMap<Notification, NotificationDto>();

        CreateMap<Notification, NotificationListDto>();
    }
}
