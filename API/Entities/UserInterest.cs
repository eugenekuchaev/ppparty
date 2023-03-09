using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
	public class UserInterest
	{
		public int Id { get; set; }
		
		[StringLength(32, MinimumLength = 1)]
		public string InterestName { get; set; } = null!;

		// Navigation properties
		public ICollection<AppUser>? AppUsers { get; set; }
	}
}