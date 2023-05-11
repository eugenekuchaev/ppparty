using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class ChangePasswordDto
	{
		public string? OldPassword { get; set; } 
		
		[StringLength(64, MinimumLength = 8)]
		[Required]
		public string? NewPassword { get; set; }
		
		[StringLength(64, MinimumLength = 8)]
		[Required]
		public string? ConfirmNewPassword { get; set; }
	}
}