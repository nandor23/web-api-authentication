using Microsoft.IdentityModel.Tokens;

namespace Shared.Settings;
public class RefreshTokenSettings
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ClockSkewInSeconds { get; set; }
    public int ExpirationInDays { get; set; }
    public string SigningAlgorithm { get; set; } = SecurityAlgorithms.HmacSha512;
}