using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventory;

    public InventoryController(IInventoryService inventory) => _inventory = inventory;

    [Authorize]
    [HttpGet("seeds")]
    public async Task<IActionResult> GetSeeds()
    {
        var userId = GetUserId();
        var seeds = await _inventory.GetSeedsAsync(userId);
        return Ok(seeds);
    }

    [Authorize]
    [HttpGet("crops")]
    public async Task<IActionResult> GetCrops()
    {
        var userId = GetUserId();
        var crops = await _inventory.GetCropsAsync(userId);
        return Ok(crops);
    }

    [Authorize]
    [HttpPost("sell")]
    public async Task<IActionResult> Sell([FromBody] SellCropRequest req)
    {
        var userId = GetUserId();
        var res = await _inventory.SellCropAsync(userId, req);
        return Ok(res);
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value ?? throw new DomainException("Invalid token", 401);
        return Guid.Parse(sub);
    }
}