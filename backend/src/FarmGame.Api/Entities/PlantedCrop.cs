namespace FarmGame.Api.Entities
{
    public class PlantedCrop
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public int CropTypeId { get; set; }
        public DateTime PlantedAt { get; set; }
        public DateTime ReadyAt { get; set; }
        public DateTime? HarvestedAt { get; set; }
        public string Status { get; set; } // "Planted", "Ready", "Harvested"

        public Field Field { get; set; }
        public CropType CropType { get; set; }
    }
}