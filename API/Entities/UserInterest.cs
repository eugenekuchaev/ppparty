namespace API.Entities
{
    public class UserInterest
    {
        public int Id { get; set; }
        public string InterestName { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<AppUser>? AppUsers { get; set; }
    }
}