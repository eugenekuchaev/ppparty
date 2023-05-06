using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs
{
	public class EventDto
	{
		// Base properties
		public int Id { get; set; }

		[StringLength(64, MinimumLength = 3)]
		[Required]
		public string? EventName { get; set; }

		[StringLength(1000, MinimumLength = 100)]
		[Required]
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
		[StringLength(56)]
		[Required]
		public string? Country { get; set; }

		[StringLength(35)]
		[Required]
		public string? Region { get; set; }

		[StringLength(58)]
		[Required]
		public string? City { get; set; }

		[StringLength(100)]
		[Required]
		public string? Address { get; set; }

		// Payment properties
		public string? Currency { get; set; }
		public decimal Price { get; set; }

		// Navigation properties
		public ICollection<EventTag>? EventTags { get; set; }
		public string? EventTagsString { get; set; }
		public string? EventOwnerUsername { get; set; }
		public ICollection<EventDate>? EventDates { get; set; }
	}
}