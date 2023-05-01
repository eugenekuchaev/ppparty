using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
	public class AppUser : IdentityUser<int>
	{
		// Registration properties
		public string FullName { get; set; } = null!;
		public bool ShowEmail { get; set; }
		
		// Location properties
		public string? Country { get; set; }	
		public string? Region { get; set; }
		public string City { get; set; } = "Somewhere";

		// Personal information properties
		public string? About { get; set; }

		// Contacts properties
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
		public List<UserInterest>? UserInterests { get; set; }
		public ICollection<AppUserFriend>? AddedToFriendsByUsers { get; set; }
		public ICollection<AppUserFriend>? AddedToFriendsUsers { get; set; }
		public ICollection<Message>? MessagesSent { get; set; }
		public ICollection<Message>? MessagesRecieved { get; set; }
		public ICollection<AppUserRole> UserRoles { get; set; } = null!;
		public ICollection<Event>? OwnedEvents { get; set; }
		public ICollection<Event>? ParticipateInEvents { get; set; }
		public ICollection<Event>? InvitedToEvents { get; set; }
		public ICollection<EventNotification>? EventNotifications { get; set; }
	}
}