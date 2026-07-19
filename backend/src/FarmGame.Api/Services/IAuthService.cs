using System;

namespace FarmGame.Api.Service
{

    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        string GenerateJwtToken(Guid userId, string login, out DateTime expiresAt);
    }
}