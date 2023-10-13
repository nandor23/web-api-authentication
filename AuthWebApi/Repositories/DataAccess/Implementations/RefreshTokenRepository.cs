using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Data;
using Shared.Exceptions;

namespace Repositories.DataAccess.Implementations;
public class RefreshTokenRepository : Interfaces.IRefreshTokenRepository
{
    private readonly DatabaseContext _dbContext;

    public RefreshTokenRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new RepositoryException();
        }
    }

    public async Task<RefreshToken?> FindRefreshTokenByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(c => c!.Id == id);
        }
        catch (Exception)
        {
            throw new RepositoryException();
        }
    }

    public async Task DeleteRefreshTokenByIdAsync(Guid id)
    {
        try
        {
            var token = await _dbContext.RefreshTokens.FindAsync(id);
            _dbContext.RefreshTokens.Remove(token!);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new RepositoryException();
        }
    }

    public async Task DeleteAllRefreshTokenByUserIdAsync(Guid userId)
    {
        try
        {
            var tokensToDelete = _dbContext.RefreshTokens
                .Where(token => token!.UserId == userId);

            _dbContext.RefreshTokens.RemoveRange(tokensToDelete);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new RepositoryException();
        }
    }
}