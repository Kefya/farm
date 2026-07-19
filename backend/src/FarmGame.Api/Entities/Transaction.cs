namespace FarmGame.Api.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } // seed_buy, field_buy, crop_sell
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Metadata { get; set; } // json
    }
}