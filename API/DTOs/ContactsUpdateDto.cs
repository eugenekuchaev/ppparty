using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class ContactsUpdateDto
	{
		[StringLength(21)]
		public string? PhoneNumber { get; set; }
		
		[StringLength(80)]
		public string? FacebookLink { get; set; }
		
		[StringLength(80)]
		public string? InstagramLink { get; set; }
		
		[StringLength(80)]
		public string? TwitterLink { get; set; }
		
		[StringLength(80)]
		public string? LinkedInLink { get; set; }
		
		[StringLength(80)]
		public string? WebsiteLink { get; set; }
	}
}