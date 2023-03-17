namespace API.Entities
{
	public class AppUser
	{
		public int Id { get; set; }

		// Registration properties
		public string FullName { get; set; } = null!;
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public bool ShowEmail { get; set; }
		public byte[] PasswordHash { get; set; } = null!;
		public byte[] PasswordSalt { get; set; } = null!;

		// Location properties
		public string? Country { get; set; }	
		public string? Region { get; set; }
		public string City { get; set; } = "Somewhere";

		// Personal information properties
		public string? About { get; set; }

		// Contacts properties
		public string? PhoneNumber { get; set; }
		public string? FacebookLink { get; set; }
		public string? InstagramLink { get; set; }
		public string? TwitterLink { get; set; }
		public string? LinkedInLink { get; set; }
		public string? WebsiteLink { get; set; }

		// System properties
		public double Rating { get; set; } = 0.0;
		// Doesn't have any functionality at the time, but can be useful in the future
		public DateTime Created { get; set; } = DateTime.Now;
		public DateTime LastActive { get; set; } = DateTime.Now;

		// Navigation properties
		public UserPhoto UserPhoto { get; set; } = null!;
		public ICollection<UserInterest>? UserInterests { get; set; }
		public ICollection<AppUserFriend>? AddedToFriendsByUsers { get; set; }
		public ICollection<AppUserFriend>? AddedToFriendsUsers { get; set; }
	}
}