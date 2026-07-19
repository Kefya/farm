namespace FarmGame.Api.DTOs.Fields
{
    public class HarvestResponse
    {
        public int CropTypeId { get; set; }
        public string CropName { get; set; }
        public DateTime PlantedAt { get; set; }
        public DateTime ReadyAt { get; set; }
        public DateTime HarvestedAt { get; set; }
    }
}