using Models;
using Shared.Results;

namespace Services.Interfaces;
public interface ILogoutService
{
    Task LogoutUserAsync(string? refreshToken);

}