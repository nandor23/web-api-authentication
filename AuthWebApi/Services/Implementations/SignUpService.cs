using Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;
using Helpers.Interfaces;
using Shared.Exceptions.AuthExceptions;
using Validators.Utilities;
using Validators.Validators;


namespace Services.Implementations;
public class SignUpService : ISignUpService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserValidator<User> _userNameValidator;
    private readonly IEmailValidator<User> _emailValidator;
    private readonly IPasswordValidator<User> _passwordValidator;
    private readonly ErrorCodesCategory _errorCodes;
    private readonly IEmailEncryptionHelper _emailEncryptionHelper;
    private readonly ILogger<SignUpService> _logger;


    public SignUpService(UserManager<User> userManager, IUserValidator<User> userNameValidator, IEmailValidator<User> emailValidator,
        IPasswordValidator<User> passwordValidator, ErrorCodesCategory errorCodes, IEmailEncryptionHelper emailEncryptionHelper, ILogger<SignUpService> logger)
    {
        _userManager = userManager;
        _userNameValidator = userNameValidator;
        _emailValidator = emailValidator;
        _passwordValidator = passwordValidator;
        _errorCodes = errorCodes;
        _emailEncryptionHelper = emailEncryptionHelper;
        _logger = logger;
    }

    public async Task SignUpUserAsync(User user, string password)
    {
        var userNameError = ExtractUserNameError(await _userNameValidator.ValidateAsync(_userManager, user));

        if (userNameError is not null)
        {
            throw new SignUpException(userNameError);
        }

        var encryptedEmail = _emailEncryptionHelper.EncryptEmail(user.Email!);
        var emailError = ExtractEmailError(await _emailValidator.ValidateAsync(_userManager, user.Email!, encryptedEmail));

        if (emailError is not null)
        {
            throw new SignUpException(emailError);
        }

        var passwordError = ExtractPasswordError(await _passwordValidator.ValidateAsync(_userManager, null!, password));

        if (passwordError is not null)
        {
            throw new SignUpException(passwordError);
        }

        // The validation has been successfully passed, and the account can now be created.
        user.Email = encryptedEmail;
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            return;
        }
        throw new SignUpException("Account creation failed due to a server error");
    }

    private string? ExtractUserNameError(IdentityResult result)
    {
        var error = result.Errors.FirstOrDefault();

        if (error is not null && _errorCodes.IsUserNameErrorCode(error.Code))
        { 
            return error.Description;
        }

        return null;
    }

    private string? ExtractEmailError(IdentityResult result)
    {
        var error = result.Errors.FirstOrDefault();

        if (error is not null && _errorCodes.IsEmailErrorCode(error.Code))
        {
            return error.Description;
        }

        return null;
    }

    private string? ExtractPasswordError(IdentityResult result)
    {
        var error = result.Errors.FirstOrDefault();

        if (error is not null && _errorCodes.IsPasswordErrorCode(error.Code))
        {
            return error.Description;
        }

        return null;
    }
}
