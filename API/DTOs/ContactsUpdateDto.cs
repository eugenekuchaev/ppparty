namespace API.DTOs
{
    public class ContactsUpdateDto
    {
        public string? PhoneNumber { get; set; }
		public string? FacebookLink { get; set; }
		public string? InstagramLink { get; set; }
		public string? TwitterLink { get; set; }
		public string? LinkedInLink { get; set; }
		public string? WebsiteLink { get; set; }
    }
}