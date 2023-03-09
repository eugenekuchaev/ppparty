using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class RegisterDto
	{
		[StringLength(32, MinimumLength = 3)]
		[Required]
		public string FullName { get; set; } = null!;
		
		[EmailAddress]
		[Required]
		[StringLength(128)]
		public string Email { get; set; } = null!;
		
		[StringLength(32, MinimumLength = 3)]
		[Required]
		public string Username { get; set; } = null!;
		
		[StringLength(64, MinimumLength = 8)]
		[Required]
		public string Password { get; set; } = null!;
	}
}