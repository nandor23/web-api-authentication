using Helpers.Interfaces;
using Microsoft.Extensions.Logging;
using Repositories.DataAccess.Interfaces;
using Services.Interfaces;
using Shared.Exceptions.AuthExceptions;
using Shared.Results;

namespace Services.Implementations;
public class TokenRotationService : ITokenRotationService
{
    private readonly IAccessTokenHelper _accessTokenHelper;
    private readonly IRefreshTokenHelper _refreshTokenHelper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<TokenRotationService> _logger;

    public TokenRotationService(IAccessTokenHelper accessTokenHelper, IRefreshTokenHelper refreshTokenHelper,
        IRefreshTokenRepository refreshTokenRepository, ILogger<TokenRotationService> logger)
    {
        _accessTokenHelper = accessTokenHelper;
        _refreshTokenHelper = refreshTokenHelper;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task<TokensResult> RotateTokenAsync(string? refreshToken)
    {
        const string errorMessage = "Invalid refresh token";

        if (refreshToken is null)
        {
            throw new TokenRotationException(errorMessage);
        }

        var isValid = _refreshTokenHelper.ValidateRefreshToken(refreshToken);
        var claims = _refreshTokenHelper.GetRefreshTokenClaims(refreshToken) ?? throw new TokenRotationException(errorMessage);

        if (await _refreshTokenRepository.FindRefreshTokenByIdAsync(claims.Jti) is null)
        {
            throw new TokenRotationException(errorMessage);
        }

        // If a token is invalid, it may indicate that it is an older one that has been compromised. In such a scenario, it becomes
        // necessary to invalidate all tokens associated with the user, prompting them to log in again for enhanced security.
        // Invalidation is implemented by deleting all of the user's refresh tokens.
        if (!isValid)
        {
            await _refreshTokenRepository.DeleteAllRefreshTokenByUserIdAsync(claims.UserId);
            throw new TokenRotationException(errorMessage);
        }

        await _refreshTokenRepository.DeleteRefreshTokenByIdAsync(claims.Jti);
        var refreshTokenEntity = _refreshTokenHelper.GenerateRefreshTokenEntity(claims.UserId);
        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);

        return new TokensResult()
        {
            AccessToken = _accessTokenHelper.GenerateAccessToken(claims.UserId),
            RefreshToken = _refreshTokenHelper.GenerateRefreshToken(claims.UserId, refreshTokenEntity.Id)
        };

    }

}
