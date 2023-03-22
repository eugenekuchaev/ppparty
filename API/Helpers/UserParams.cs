namespace API.Helpers
{
	public class UserParams : PaginationParams
	{
		public string? CurrentUsername { get; set; } 
		public string? FullName { get; set; }
		public string? Username { get; set; }
		public string? Country { get; set; }
		public string? Region { get; set; }
		public string? City { get; set; }
		public string? UserInterest { get; set; }
	}
}