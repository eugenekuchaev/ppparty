using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class LocationUpdateDto
	{
		[StringLength(56)]
		public string? Country { get; set; } 
		
		[StringLength(35)]
		public string? Region { get; set; }
		
		[StringLength(58)]
		public string City { get; set; } = "Somewhere";
	}
}