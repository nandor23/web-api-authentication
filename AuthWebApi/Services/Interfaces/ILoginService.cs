using Models;
using Shared.Results;

namespace Services.Interfaces;
public interface ILoginService
{
    Task<TokensResult> LoginUserAsync(User user, string password);
}