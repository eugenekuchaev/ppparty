using API.Entities;

namespace API.DTOs
{
	public class FullEventDto
	{
		// Base properties
		public int Id { get; set; }
		public string? EventName { get; set; } 
		public string? Description { get; set; } 
		public string? EventPhotoUrl { get; set; }
		
		// Time properties
		public bool IsEnded
		{
			get
			{
				if (IsCancelled == true)
				{
					return true;
				}
				
				var latestDate = EventDates!.Max(ed => ed.EndDate);
				return latestDate < DateTime.UtcNow;
			}
		}
		
		public bool IsCancelled { get; set; } = false;
		
		// Location properties
		public string? Country { get; set; } 
		public string? Region { get; set; }
		public string? City { get; set; } 
		public string? Address { get; set; } 
		
		// Payment properties
		public string? Currency { get; set; } 
		public decimal Price { get; set; } = 0.0m;
		
		// Navigation properties
		public EventPhoto? EventPhoto { get; set; } 
		public ICollection<EventTag>? EventTags { get; set; } 
		public MemberInEventDto? EventOwner { get; set; } 
		public ICollection<MemberInEventDto>? Participants { get; set; } 
		public ICollection<MemberInEventDto>? FriendsParticipants { get; set; }
		public ICollection<EventDate>? EventDates { get; set; } 
	}
}