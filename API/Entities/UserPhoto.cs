using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("UserPhotos")]
    public class UserPhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = default!;
        public int AppUserId { get; set; }
    }
}