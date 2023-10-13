using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Helpers.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Results;
using Shared.Settings;


namespace Helpers.Implementations;
public class AccessTokenHelper : IAccessTokenHelper
{
    private readonly AccessTokenSettings _accessTokenSettings;

    public AccessTokenHelper(IOptions<AccessTokenSettings> accessTokenSettings)
    {
        _accessTokenSettings = accessTokenSettings.Value;
    }

    public string GenerateAccessToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim(JwtClaimTypes.UserId, userId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _accessTokenSettings.Issuer,
            Audience = _accessTokenSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_accessTokenSettings.ExpirationInMinutes),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenSettings.SecretKey)), _accessTokenSettings.SigningAlgorithm),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateAccessToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = _accessTokenSettings.Issuer,
            ValidAudience = _accessTokenSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenSettings.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidAlgorithms = new[] { _accessTokenSettings.SigningAlgorithm },
            ClockSkew = TimeSpan.FromSeconds(_accessTokenSettings.ClockSkewInSeconds),
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

    public JwtClaimsResult? GetAccessTokenClaims(string token)
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
