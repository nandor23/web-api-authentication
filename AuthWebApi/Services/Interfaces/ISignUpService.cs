using Models;

namespace Services.Interfaces;
public interface ISignUpService
{
    Task SignUpUserAsync(User user, string password);
}