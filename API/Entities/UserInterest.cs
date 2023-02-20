namespace API.Entities
{
    public class UserInterest
    {
        public int Id { get; set; }
        public string InterestName { get; set; } = null!;

        // Navigation properties
        public ICollection<AppUser>? AppUsers { get; set; }
    }
}