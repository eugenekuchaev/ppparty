namespace API.Entities
{
	public class EventDate
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int EventId { get; set; }
		public Event? Event { get; set; } 
	}
}