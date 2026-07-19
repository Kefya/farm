using System.ComponentModel.DataAnnotations;

namespace FarmGame.Api.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public decimal Balance { get; set; } = 0m;
        public decimal TotalEarned { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Farm Farm { get; set; }
    }
}