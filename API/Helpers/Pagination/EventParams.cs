namespace API.Helpers
{
	public class EventParams : PaginationParams
	{
		public string? EventName { get; set; } 
		public string? FromDate { get; set; }
		public string? ToDate { get; set; }
		public string? Country { get; set; }
		public string? Region { get; set; }
		public string? City { get; set; }
		public string? EventTag { get; set; }
	}
}