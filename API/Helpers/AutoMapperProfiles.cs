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
		}
	}
}