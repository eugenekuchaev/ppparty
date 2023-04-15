using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class EventTag
    {
        public int Id { get; set; }
		
		[StringLength(32, MinimumLength = 1)]
		public string EventTagName { get; set; } = null!;

		// Navigation properties
		public ICollection<Event>? Events { get; set; }
    }
}