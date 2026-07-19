using FarmGame.Api.Data;
using FarmGame.Api.DTOs.Auth;
using FarmGame.Api.Entities;
using FarmGame.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuthService _auth;

        public AuthController(ApplicationDbContext db, IAuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Login)  string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Login and password are required.");

            var exists = await _db.Users.AnyAsync(u => u.Login == req.Login);
            if (exists) return Conflict("Login already taken.");

            var user = new User
            {
                Login = req.Login,
                PasswordHash = _auth.HashPassword(req.Password),
                Balance = 100m // стартовый баланс
            };

            using var tx = await _db.Database.BeginTransactionAsync();
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            // создаём ферму и стартовые поля
            var farm = new Farm { UserId = user.Id };
            await _db.Farms.AddAsync(farm);
            await _db.SaveChangesAsync();

            // добавляем 3 стартовых поля
            for (int i = 0; i < 3; i++)
            {
                var field = new Field { FarmId = farm.Id, SlotIndex = i, BoughtAt = DateTime.UtcNow, PriceBought = 0m };
                await _db.Fields.AddAsync(field);
            }
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return CreatedAtAction(nameof(Register), new { id = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Login)  string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Login and password required.");

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Login == req.Login);
            if (user == null) return Unauthorized("Invalid login or password.");

            if (!_auth.VerifyPassword(req.Password, user.PasswordHash))
                return Unauthorized("Invalid login or password.");

            var token = _auth.GenerateJwtToken(user.Id, user.Login, out DateTime expiresAt);

            var resp = new AuthResponse { Token = token, ExpiresAt = expiresAt };
            return Ok(resp);
        }
    }
}