namespace FarmGame.Api.Entities
{
    public class Field
    {
        public Guid Id { get; set; }
        public Guid FarmId { get; set; }
        public int SlotIndex { get; set; }
        public DateTime? BoughtAt { get; set; }
        public decimal? PriceBought { get; set; }

        public Farm Farm { get; set; }
        public PlantedCrop PlantedCrop { get; set; }
    }
}