namespace API.Entities
{
    public class AppUserUserInterest
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = default!;
        
        public int UserInterestId { get; set; }
        public UserInterest UserInterest { get; set; } = default!;
    }
}