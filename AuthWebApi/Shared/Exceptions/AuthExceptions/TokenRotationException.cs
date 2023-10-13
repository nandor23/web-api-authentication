namespace Shared.Exceptions.AuthExceptions;
public class TokenRotationException : ServiceException
{
    public TokenRotationException(string message) : base(message)
    {
    }
}