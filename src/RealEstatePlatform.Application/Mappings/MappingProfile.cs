using AutoMapper;
using RealEstatePlatform.Application.DTOs.Auth;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Application.DTOs.Message;
using RealEstatePlatform.Application.DTOs.Notification;
using RealEstatePlatform.Application.DTOs.User;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping entities to DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified));

        CreateMap<ApplicationUser, UserProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<UpdateProfileDto, ApplicationUser>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

        // Notification mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.RelatedEntityId, opt => opt.MapFrom(src => src.RelatedEntityId))
            .ForMember(dest => dest.RelatedEntityType, opt => opt.MapFrom(src => src.RelatedEntityType));

        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
            .ForMember(dest => dest.SenderAvatarUrl, opt => opt.MapFrom(src => src.Sender.AvatarUrl))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt));

        // Conversation mappings
        CreateMap<Conversation, ConversationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.LastMessageAt, opt => opt.MapFrom(src => src.LastMessageAt))
            .ForMember(dest => dest.LastMessageContent, opt => opt.MapFrom(src => 
                src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()!.Content))
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));

        CreateMap<ConversationParticipant, ParticipantDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
            .ForMember(dest => dest.LastReadAt, opt => opt.MapFrom(src => src.LastReadAt));
    }
}
