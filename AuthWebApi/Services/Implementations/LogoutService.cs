using Helpers.Interfaces;
using Microsoft.Extensions.Logging;
using Repositories.DataAccess.Interfaces;
using Services.Interfaces;
using Shared.Exceptions.AuthExceptions;

namespace Services.Implementations;
public class LogoutService : ILogoutService
{
    private readonly IRefreshTokenHelper _refreshTokenHelper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<LogoutService> _logger;



    public LogoutService(IRefreshTokenHelper refreshTokenHelper, IRefreshTokenRepository refreshTokenRepository, ILogger<LogoutService> logger)
    {
        _refreshTokenHelper = refreshTokenHelper;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task LogoutUserAsync(string? refreshToken)
    {
        const string errorMessage = "Logout failed";

        if (refreshToken is null)
        {
            throw new LogoutException(errorMessage);
        }

        var isValid = _refreshTokenHelper.ValidateRefreshToken(refreshToken);

        if (!isValid)
        {
            throw new LogoutException(errorMessage);
        }

        var claims = _refreshTokenHelper.GetRefreshTokenClaims(refreshToken) ?? throw new LogoutException(errorMessage);

        if (await _refreshTokenRepository.FindRefreshTokenByIdAsync(claims.Jti) is null)
        {
            throw new LogoutException(errorMessage);
        }

        await _refreshTokenRepository.DeleteRefreshTokenByIdAsync(claims.Jti);
    }
}