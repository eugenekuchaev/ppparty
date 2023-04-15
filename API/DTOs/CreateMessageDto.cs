using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class CreateMessageDto
	{
		public string RecipientUsername { get; set; } = null!;
		public string Content { get; set; } = null!;
	}
}