using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class LeaderboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public LeaderboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> GetTop(int top = 10)
    {
        top = Math.Clamp(top, 1, 100);
        var q = await _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.Balance)
            .Take(top)
            .Select((u, idx) => new
            {
                login = u.Login,
                balance = u.Balance,
                totalEarned = u.TotalEarned
            })
            .ToListAsync();

        return Ok(q);
    }
}