using Models;

namespace Repositories.DataAccess.Interfaces;
public interface IRefreshTokenRepository
{
    Task AddRefreshTokenAsync(RefreshToken refreshToken);

    Task<RefreshToken?> FindRefreshTokenByIdAsync(Guid id);

    Task DeleteRefreshTokenByIdAsync(Guid id);

    Task DeleteAllRefreshTokenByUserIdAsync(Guid userId);
}