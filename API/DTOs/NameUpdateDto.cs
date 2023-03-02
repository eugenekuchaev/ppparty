using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class NameUpdateDto
    {
        [MinLength(4)]
		public string FullName { get; set; } = null!;
    }
}