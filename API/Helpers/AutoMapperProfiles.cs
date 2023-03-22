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
		}
	}
}