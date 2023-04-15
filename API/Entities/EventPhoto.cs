namespace API.Entities
{
    public class EventPhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public string? PublicId { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}