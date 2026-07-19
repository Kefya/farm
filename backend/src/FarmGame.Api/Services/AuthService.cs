using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FarmGame.Api.Service
{

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        public AuthService(IConfiguration config) => _config = config;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        public string GenerateJwtToken(Guid userId, string login, out DateTime expiresAt)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? throw new Exception("JWT Key is missing"));
            var expiresMinutes = int.Parse(jwtSection["ExpiresMinutes"] ?? "60");
            expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, login)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["ExpiresMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}