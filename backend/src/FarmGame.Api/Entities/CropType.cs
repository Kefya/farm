namespace FarmGame.Api.Entities
{
    public class CropType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SeedPrice { get; set; }
        public int GrowSeconds { get; set; } // время созревания в секундах
        public decimal SellPrice { get; set; }
    }
}