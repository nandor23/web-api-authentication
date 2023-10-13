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

        var encryptedEmail = _emailEncryptionHelper.EncryptEmail(user.Email!);

        var userNameValidationResult = await _userNameValidator.ValidateAsync(_userManager, user);
        var emailValidationResult = await _emailValidator.ValidateAsync(_userManager, user.Email!, encryptedEmail);
        var passwordValidationResult = await _passwordValidator.ValidateAsync(_userManager, null!, password);

        if (userNameValidationResult.Succeeded && emailValidationResult.Succeeded && passwordValidationResult.Succeeded)
        {
            user.Email = encryptedEmail;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return;
            }
            throw new SignUpException("Account creation failed due to a server error");
        }

        string? userNameMessage = null, emailMessage = null, passwordMessage = null;

        var error = userNameValidationResult.Errors.FirstOrDefault();

        if (error is not null && _errorCodes.IsUserNameErrorCode(error.Code))
        {
            userNameMessage = error.Description;
        }

        error = emailValidationResult.Errors.FirstOrDefault();

        if (error is not null && _errorCodes.IsEmailErrorCode(error.Code))
        {
            emailMessage = error.Description;
        }

        error = passwordValidationResult.Errors.FirstOrDefault();

        if (error is not null && _errorCodes.IsPasswordErrorCode(error.Code))
        {
            passwordMessage = error.Description;
        }

        throw new SignUpException(userNameMessage, emailMessage, passwordMessage);
    }
}
