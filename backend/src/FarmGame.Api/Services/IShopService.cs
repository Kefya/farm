using FarmGame.Api.DTOs.Shop;
using System.Threading.Tasks;

namespace FarmGame.Api.Services
{
    public interface IShopService
    {
        Task<IEnumerable<CropTypeDto>> GetCropTypesAsync();
        Task<BuySeedsResultDto> BuySeedsAsync(Guid userId, int cropTypeId, int quantity);
    }
}