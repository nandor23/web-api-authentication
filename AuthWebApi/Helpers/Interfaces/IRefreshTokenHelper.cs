using Models;
using Shared.Results;

namespace Helpers.Interfaces;
public interface IRefreshTokenHelper
{
    string GenerateRefreshToken(Guid userId, Guid tokenId);

    RefreshToken GenerateRefreshTokenEntity(Guid userId);

    bool ValidateRefreshToken(string token);

    JwtClaimsResult? GetRefreshTokenClaims(string token);
}