using FarmGame.Api.DTOs.Fields;
using System.Threading.Tasks;

namespace FarmGame.Api.Services
{
    public interface IFieldService
    {
        Task<IEnumerable<FieldDto>> GetFieldsAsync(Guid userId);
        Task<OperationResultDto> PlantAsync(Guid userId, Guid fieldId, int cropTypeId);
        Task<OperationResultDto> HarvestAsync(Guid userId, Guid fieldId);
        Task<OperationResultDto> BuyFieldAsync(Guid userId);
    }
}