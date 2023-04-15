using API.Entities;

namespace API.DTOs
{
	public class MemberDto
	{
		public int Id { get; set; }

		// Registration properties
		public string FullName { get; set; } = null!;
		public string Username { get; set; } = null!;
		public string Email { get; set; } = null!;

		// Location properties
		public string? Country { get; set; }	
		public string? Region { get; set; }
		public string? City { get; set; }

		// Personal information properties
		public string? About { get; set; }
		public string? UserPhotoUrl { get; set; }

		// Contacts properties
		public string? PhoneNumber { get; set; }
		public string? FacebookLink { get; set; }
		public string? InstagramLink { get; set; }
		public string? TwitterLink { get; set; }
		public string? LinkedInLink { get; set; }
		public string? WebsiteLink { get; set; }

		// System properties
		public double Rating { get; set; }
		// Doesn't have any functionality at the time, but can be useful in the future
		public DateTime Created { get; set; } 
		public DateTime LastActive { get; set; } 

		// Navigation properties
		public UserPhoto? UserPhoto { get; set; }
		public ICollection<UserInterest>? UserInterests { get; set; } 
	}
}