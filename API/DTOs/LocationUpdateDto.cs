namespace API.DTOs
{
	public class LocationUpdateDto
	{
		public string? Country { get; set; } 
		public string? Region { get; set; }
		public string City { get; set; } = "Somewhere";
	}
}