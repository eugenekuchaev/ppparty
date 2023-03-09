using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class NameUpdateDto
    {
        [StringLength(32, MinimumLength = 3)]
		public string FullName { get; set; } = null!;
    }
}