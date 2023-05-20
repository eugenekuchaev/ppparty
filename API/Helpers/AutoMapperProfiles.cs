using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<AppUser, MemberDto>()
				.ForMember(dest => dest.UserPhotoUrl,
					opt => opt.MapFrom(src => src.UserPhoto.PhotoUrl));

			CreateMap<UserPhoto, UserPhotoDto>();
			CreateMap<UserInterest, UserInterestDto>();
			CreateMap<NameUpdateDto, AppUser>();
			CreateMap<LocationUpdateDto, AppUser>();
			CreateMap<AboutUpdateDto, AppUser>();
			CreateMap<ContactsUpdateDto, AppUser>();
			CreateMap<RegisterDto, AppUser>();

			CreateMap<Message, MessageDto>()
				.ForMember(dest => dest.SenderPhotoUrl,
					opt => opt.MapFrom(src => src.Sender.UserPhoto.PhotoUrl))
				.ForMember(dest => dest.RecipientPhotoUrl,
					opt => opt.MapFrom(src => src.Recipient.UserPhoto.PhotoUrl));

			CreateMap<Event, EventDto>()
				.ForMember(dest => dest.EventPhotoUrl,
					opt => opt.MapFrom(src => src.EventPhoto.PhotoUrl))
				.ForMember(dest => dest.EventOwnerUsername,
					opt => opt.MapFrom(src => src.EventOwner.UserName));

			CreateMap<Event, FullEventDto>()
				.ForMember(dest => dest.EventPhotoUrl,
					opt => opt.MapFrom(src => src.EventPhoto.PhotoUrl));

			CreateMap<EventUpdateDto, Event>()
				.ForMember(dest => dest.EventDates, opt => opt.Ignore());

			CreateMap<EventDto, Event>();
			CreateMap<EventPhoto, EventPhotoDto>();

			CreateMap<AppUser, MemberInEventDto>()
				.ForMember(dest => dest.UserPhotoUrl,
					opt => opt.MapFrom(src => src.UserPhoto.PhotoUrl));
		}
	}
}