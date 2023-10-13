using Shared.Results;

namespace Services.Interfaces;
public interface ITokenRotationService
{
    Task<TokensResult> RotateTokenAsync(string? refreshToken);
}