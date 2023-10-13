using Shared.Errors;

namespace Shared.Exceptions.AuthExceptions;
public class SignUpException : ServiceException
{
    public SignUpException(string? userNameMessage, string? emailMessage, string? passwordMessage) :
        base(new SignUpError(userNameMessage, emailMessage, passwordMessage))
    {
    }

    public SignUpException(string message) : base(message)
    {
    }
}