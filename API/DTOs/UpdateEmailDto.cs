using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateEmailDto
    {
        [EmailAddress]
		[Required]
		[StringLength(128)]
		public string? Email { get; set; } 
    }
}