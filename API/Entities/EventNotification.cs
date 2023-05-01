namespace API.Entities
{
	public class EventNotification
	{
		public int Id { get; set; }
		public DateTime TimeStamp { get; set; } = DateTime.Now;
		public string NotificationMessage { get; set; } = null!;
		public int EventId { get; set; }
		public string EventName { get; set; } = null!;
		public bool Read { get; set; } = false;
		public ICollection<AppUser> Recipients { get; set; } = null!;
	}
}