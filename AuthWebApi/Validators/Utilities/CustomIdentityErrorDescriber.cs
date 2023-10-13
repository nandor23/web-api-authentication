using Microsoft.AspNetCore.Identity;

namespace Validators.Utilities;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError InvalidUserName(string? userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = "Username can only include letters, numbers, underscores ( _ ), and periods ( . )"
        };
    }

    public override IdentityError DuplicateUserName(string? userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = "Username is already taken"
        };
    }

    public IdentityError UserNameTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(UserNameTooShort),
            Description = $"Username must be at least {length} characters"
        };
    }

    public IdentityError UserNameTooLong(int length)
    {
        return new IdentityError
        {
            Code = nameof(UserNameTooLong),
            Description = $"Username cannot be longer than {length} characters"
        };
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = "Wrong or Invalid email address"
        };
    }

    public override IdentityError DuplicateEmail(string? email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = "Email address is already in use"
        };
    }

    public IdentityError InvalidPassword()
    {
        return new IdentityError
        {
            Code = nameof(InvalidPassword),
            Description = "Password must include letters, a number, and a special character"
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Password must be at least {length} characters"
        };
    }

    public IdentityError PasswordTooLong(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooLong),
            Description = $"Password cannot be longer than {length} characters"
        };
    }
}
