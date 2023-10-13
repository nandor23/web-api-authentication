namespace Shared.Exceptions.AuthExceptions;
public class LogoutException : ServiceException
{
    public LogoutException(string message) : base(message)
    {
    }
}