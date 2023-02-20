namespace API.Entities
{
    public class UserPhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public string? PublicId { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}