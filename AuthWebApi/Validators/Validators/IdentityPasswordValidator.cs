using Microsoft.AspNetCore.Identity;
using Models;
using Validators.Options;
using Validators.Utilities;

namespace Validators.Validators;
public class IdentityPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : User
{
    private readonly CustomIdentityOptions _options;
    private readonly CustomIdentityErrorDescriber _describer;

    public IdentityPasswordValidator(CustomIdentityOptions options, CustomIdentityErrorDescriber describer)
    {
        _options = options;
        _describer = describer;
    }

    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser? user, string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        if (password.Length < _options.Password.MinLength)
        {
            return Task.FromResult(IdentityResult.Failed(_describer.PasswordTooShort(_options.Password.MinLength)));
        }

        if (password.Length > _options.Password.MaxLength)
        {
            return Task.FromResult(IdentityResult.Failed(_describer.PasswordTooLong(_options.Password.MaxLength)));
        }

        if (_options.Password.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        if (_options.Password.RequireAnyLetter && !password.Any(IsLetter))
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        if (_options.Password.RequireDigit && !password.Any(IsDigit))
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        if (_options.Password.RequireLowercase && !password.Any(IsLower))
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        if (_options.Password.RequireUppercase && !password.Any(IsUpper))
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        if (_options.Password.RequiredUniqueChars >= 1 && password.Distinct().Count() < _options.Password.RequiredUniqueChars)
        {
            return Task.FromResult(IdentityResult.Failed(_describer.InvalidPassword()));
        }

        return Task.FromResult(IdentityResult.Success);
    }

    private static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }
    private static bool IsLower(char c)
    {
        return c is >= 'a' and <= 'z';
    }
    private static bool IsUpper(char c)
    {
        return c is >= 'A' and <= 'Z';
    }
    private static bool IsLetterOrDigit(char c)
    {
        return IsUpper(c) || IsLower(c) || IsDigit(c);
    }
    private static bool IsLetter(char c)
    {
        return IsUpper(c) || IsLower(c);
    }
}
