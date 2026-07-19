using FarmGame.Api.DTOs.Shop;
using FarmGame.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace FarmGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shop;
        public ShopController(IShopService shop) => _shop = shop;

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"));

        [HttpPost("seeds/buy")]
        public async Task<IActionResult> BuySeeds([FromBody] BuySeedRequest req)
        {
            try
            {
                var userId = GetUserId();
                await _shop.BuySeedsAsync(userId, req.CropTypeId, req.Quantity);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}