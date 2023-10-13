namespace Shared.Exceptions.AuthExceptions;
public class LoginException : ServiceException
{
    public LoginException(string message) : base(message)
    {
    }
}