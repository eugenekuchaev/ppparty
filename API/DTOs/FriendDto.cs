namespace API.DTOs
{
	public class FriendDto
	{
		public int Id { get; set; }
		public string Username { get; set; } = null!;
		public string FullName { get; set; } = null!;
		public string UserPhotoUrl { get; set; } = null!;
		public string City { get; set; } = null!;
	}
}