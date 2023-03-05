namespace API.DTOs
{
	public class UserDto
	{
		public string Username { get; set; } = null!;
		public string Token { get; set; } = null!;
		public string? PhotoUrl { get; set; } 
	}
}