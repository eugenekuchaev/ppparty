namespace API.Entities
{
    public class UserPhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = default!;
    }
}