using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class ChangeEmailDto
    {
        [EmailAddress]
		[Required]
		[StringLength(128)]
		public string? Email { get; set; } 
    }
}