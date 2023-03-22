namespace API.Entities
{
	public class Message
	{
		public int Id { get; set; }
		public int SenderId { get; set; }
		public string SenderUsername { get; set; } = null!;
		public AppUser Sender { get; set; } = null!;
		public int RecipientId { get; set; }
		public string RecipientUsername { get; set; } = null!;
		public AppUser Recipient { get; set; } = null!;
		public string Content { get; set; } = null!;
		public DateTime? DateRead { get; set; }
		public DateTime MessageSent { get; set; } = DateTime.Now;
		public bool SenderDeleted { get; set; }
		public bool RecipientDeleted { get; set; }
	}
}