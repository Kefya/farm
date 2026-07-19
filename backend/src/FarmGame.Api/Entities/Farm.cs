namespace FarmGame.Api.Entities
{
    public class Farm
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public ICollection<Field> Fields { get; set; }
    }
}