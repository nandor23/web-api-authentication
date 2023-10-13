using Microsoft.AspNetCore.Identity;

namespace Validators.Validators;
public interface IEmailValidator<TUser> where TUser : class
{
    Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, string email, string encryptedEmail);
}