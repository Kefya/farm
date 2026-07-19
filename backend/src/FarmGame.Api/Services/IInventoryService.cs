using FarmGame.Api.DTOs.Inventory;

namespace FarmGame.Api.Services
{
    public interface IInventoryService
    {
        Task<OperationResult> SellCropAsync(Guid userId, SellCropRequest req);
        Task<IEnumerable<InventorySeed>> GetSeedAsync(Guid userId);
        Task<IEnumerable<InventoryCrop>> GetCropAsync(Guid userId);
    }
}