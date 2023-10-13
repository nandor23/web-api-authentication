namespace Validators.Utilities;
public class ErrorCodesCategory
{
    private readonly CustomIdentityErrorDescriber _errorDescriber;
    private readonly HashSet<string> _userNameErrorCodes;
    private readonly HashSet<string> _emailErrorCodes;
    private readonly HashSet<string> _passwordErrorCodes;

    public ErrorCodesCategory(CustomIdentityErrorDescriber errorDescriber)
    {
        _errorDescriber = errorDescriber;

        _userNameErrorCodes = new HashSet<string>
        {
            nameof(_errorDescriber.InvalidUserName),
            nameof(_errorDescriber.DuplicateUserName),
            nameof(_errorDescriber.UserNameTooShort),
            nameof(_errorDescriber.UserNameTooLong),
        };
        _emailErrorCodes = new HashSet<string>
        {
            nameof(_errorDescriber.InvalidEmail),
            nameof(_errorDescriber.DuplicateEmail),
        };
        _passwordErrorCodes = new HashSet<string>
        {
            nameof(_errorDescriber.InvalidPassword),
            nameof(_errorDescriber.PasswordTooShort),
            nameof(_errorDescriber.PasswordTooLong),
        };
    }

    public bool IsUserNameErrorCode(string code)
    {
        return _userNameErrorCodes.Contains(code);
    }

    public bool IsEmailErrorCode(string code)
    {
        return _emailErrorCodes.Contains(code);
    }

    public bool IsPasswordErrorCode(string code)
    {
        return _passwordErrorCodes.Contains(code);
    }
}