using FarmGame.Api.DTOs.Fields;
using FarmGame.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FieldsController : ControllerBase
    {
        private readonly IFieldService _field;
        public FieldsController(IFieldService field) => _field = field;

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"));

        [HttpPost("{fieldId}/plant")]
        public async Task<IActionResult> Plant(Guid fieldId, [FromBody] PlantRequest req)
        {
            try
            {
                var userId = GetUserId();
                await _field.PlantAsync(userId, fieldId, req.CropTypeId);
                return Ok();
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("{fieldId}/harvest")]
        public async Task<IActionResult> Harvest(Guid fieldId)
        {
            try
            {
                var userId = GetUserId();
                var res = await _field.HarvestAsync(userId, fieldId);
                return Ok(res);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}