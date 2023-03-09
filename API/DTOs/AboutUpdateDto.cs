using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class AboutUpdateDto
	{
		[StringLength(500)]
		public string? About { get; set; }
	}
}