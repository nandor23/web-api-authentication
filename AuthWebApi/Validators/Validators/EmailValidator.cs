using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Text.RegularExpressions;
using Models;
using Validators.Options;
using Validators.Utilities;

namespace Validators.Validators;
public class EmailValidator<TUser> : IEmailValidator<TUser> where TUser : User
{
    private readonly CustomIdentityOptions _options;
    private readonly CustomIdentityErrorDescriber _describer;

    public EmailValidator(CustomIdentityOptions options, CustomIdentityErrorDescriber describer)
    {
        _options = options;
        _describer = describer;
    }

    public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, string email, string encryptedEmail)
    {
        if (!IsValidEmail(email))
        {
            return IdentityResult.Failed(_describer.InvalidEmail(email));
        }

        if (_options.User.RequireUniqueEmail && await manager.FindByEmailAsync(encryptedEmail) is not null)
        {
            return IdentityResult.Failed(_describer.DuplicateEmail(email));
        }

        return IdentityResult.Success;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}
