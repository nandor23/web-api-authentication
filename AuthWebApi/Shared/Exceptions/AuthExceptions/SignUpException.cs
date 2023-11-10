namespace Shared.Exceptions.AuthExceptions;
public class SignUpException : ServiceException
{
    public SignUpException(string message) : base(message)
    {
    }
}