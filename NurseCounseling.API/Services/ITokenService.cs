using NurseCounseling.API.Models;
using System.Security.Claims;

namespace NurseCounseling.API.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, string roleName);
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<RefreshToken> GetRefreshTokenAsync(string userId);
    }
}
