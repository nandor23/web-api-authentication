using Microsoft.AspNetCore.Identity;
using Models;
using Validators.Options;
using Validators.Utilities;

namespace Validators.Validators;
public class IdentityUserNameValidator<TUser> : IUserValidator<TUser> where TUser : User
{
    private readonly CustomIdentityOptions _options;
    private readonly CustomIdentityErrorDescriber _describer;

    public IdentityUserNameValidator(CustomIdentityOptions options, CustomIdentityErrorDescriber describer)
    {
        _options = options;
        _describer = describer;
    }

    public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var userName = user.UserName;
        if (string.IsNullOrWhiteSpace(userName))
        {
            return IdentityResult.Failed(_describer.InvalidUserName(userName));
        }

        if (userName.Length < _options.User.MinUserNameLength)
        {
            return IdentityResult.Failed(_describer.UserNameTooShort(_options.User.MinUserNameLength));
        }

        if (userName.Length > _options.User.MaxUserNameLength)
        {
            return IdentityResult.Failed(_describer.UserNameTooLong(_options.User.MaxUserNameLength));
        }

        if (!string.IsNullOrEmpty(_options.User.AllowedUserNameCharacters) && userName.Any(c => !_options.User.AllowedUserNameCharacters.Contains(c)))
        {
            return IdentityResult.Failed(_describer.InvalidUserName(userName));
        }

        if (await manager.FindByNameAsync(userName) != null)
        {
            return IdentityResult.Failed(_describer.DuplicateUserName(userName));
        }

        return IdentityResult.Success;
    }
}