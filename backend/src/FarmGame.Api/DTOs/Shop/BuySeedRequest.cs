namespace FarmGame.Api.DTOs.Shop
{
    public class BuySeedRequest
    {
        public int CropTypeId { get; set; }
        public int Quantity { get; set; }
    }
}