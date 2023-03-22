namespace API.DTOs
{
	public class ConversationDto
	{
		public string FriendFullName { get; set; } = null!;
		public string FriendUsername { get; set; } = null!;
		public string FriendPhotoUrl { get; set; } = null!;
		public string LastMessageAuthorName { get; set; } = null!;
		public string LastMessageContent { get; set; } = null!;
		public DateTime LastMessageSent { get; set; }
		public DateTime? LastMessageRead { get; set; }
	}
}