namespace FarmGame.Api.Entities
{
    public class InventorySeed
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int CropTypeId { get; set; }
        public int Quantity { get; set; }

        public User User { get; set; }
        public CropType CropType { get; set; }
    }
}