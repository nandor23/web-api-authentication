using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Helpers.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Results;
using Shared.Settings;
using System.Text;
using Models;

namespace Helpers.Implementations;
public class RefreshTokenHelper : IRefreshTokenHelper
{
    private readonly RefreshTokenSettings _refreshTokenSettings;

    public RefreshTokenHelper(IOptions<RefreshTokenSettings> refreshTokenSettings)
    {
        _refreshTokenSettings = refreshTokenSettings.Value;
    }

    public string GenerateRefreshToken(Guid userId, Guid tokenId)
    {
        var claims = new[]
        {
            new Claim(JwtClaimTypes.UserId, userId.ToString()),
            new Claim(JwtClaimTypes.Jti, tokenId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _refreshTokenSettings.Issuer,
            Audience = _refreshTokenSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationInDays),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenSettings.SecretKey)), _refreshTokenSettings.SigningAlgorithm),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshTokenEntity(Guid userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            CreationTime = DateTime.UtcNow,
            ExpirationTime = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationInDays),
        };
    }

    public bool ValidateRefreshToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = _refreshTokenSettings.Issuer,
            ValidAudience = _refreshTokenSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenSettings.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidAlgorithms = new[] { _refreshTokenSettings.SigningAlgorithm },
            ClockSkew = TimeSpan.FromSeconds(_refreshTokenSettings.ClockSkewInSeconds),
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }

    public JwtClaimsResult? GetRefreshTokenClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return new JwtClaimsResult
            {
                Jti = Guid.Parse(jwtToken.Claims.First(claim => claim.Type == JwtClaimTypes.Jti).Value),
                UserId = Guid.Parse(jwtToken.Claims.First(claim => claim.Type == JwtClaimTypes.UserId).Value)
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}
