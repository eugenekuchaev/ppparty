namespace API.Entities
{
	public class Event
	{
		// Base properties
		public int Id { get; set; }
		public string EventName { get; set; } = null!;
		public string Description { get; set; } = null!;
		
		// Time properties
		public bool IsEnded { get; set; } = false;
		
		// Location properties
		public string Country { get; set; } = null!;
		public string Region { get; set; } = null!;
		public string City { get; set; } = null!;
		public string Address { get; set; } = null!;
		
		// Payment properties
		public string Currency { get; set; } = null!;
		public decimal Price { get; set; } = 0.0m;
		
		// Navigation properties
		public EventPhoto EventPhoto { get; set; } = null!; 
		public ICollection<EventTag> EventTags { get; set; } = null!; 
		public int EventOwnerId { get; set; }
		public AppUser EventOwner { get; set; } = null!;
		public ICollection<AppUser> Participants { get; set; } = null!;
		public ICollection<EventDate> EventDates { get; set; } = null!;
		public ICollection<AppUser>? InvitedToEvent { get; set; }
	}
}