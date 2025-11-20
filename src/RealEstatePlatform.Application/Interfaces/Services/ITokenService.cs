using System.Security.Claims;

namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// JWT token service interface
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(string userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
