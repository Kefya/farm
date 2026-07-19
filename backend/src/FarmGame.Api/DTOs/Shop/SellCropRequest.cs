namespace FarmGame.Api.DTOs.Inventory
{
    public class SellCropRequest
    {
        public int CropTypeId { get; set; }
        public int Quantity { get; set; }
    }
}