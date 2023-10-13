namespace Shared.Errors;
public class SignUpError
{
    public string? UserName;
    public string? Email;
    public string? Password;

    public SignUpError(string? userName, string? email, string? password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }
}