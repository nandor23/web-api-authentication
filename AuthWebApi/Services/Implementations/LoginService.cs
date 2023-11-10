using Helpers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Repositories.DataAccess.Interfaces;
using Services.Interfaces;
using Shared.Exceptions.AuthExceptions;
using Shared.Results;
using Validators.Options;

namespace Services.Implementations;
public class LoginService : ILoginService
{
    private readonly UserManager<User> _userManager;
    private readonly IAccessTokenHelper _accessTokenHelper;
    private readonly IRefreshTokenHelper _refreshTokenHelper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IEmailEncryptionHelper _emailEncryptionHelper;
    private readonly CustomIdentityOptions _customIdentityOptions;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserManager<User> userManager, IAccessTokenHelper accessTokenHelper, IRefreshTokenHelper refreshTokenHelper, IRefreshTokenRepository refreshTokenRepository, 
        IEmailEncryptionHelper emailEncryptionHelper, IOptions<CustomIdentityOptions> customIdentityOptions, ILogger<LoginService> logger)
    {
        _userManager = userManager;
        _accessTokenHelper = accessTokenHelper;
        _refreshTokenHelper = refreshTokenHelper;
        _refreshTokenRepository = refreshTokenRepository;
        _emailEncryptionHelper = emailEncryptionHelper;
        _customIdentityOptions = customIdentityOptions.Value;
        _logger = logger;
    }

    public async Task<TokensResult> LoginUserAsync(User user, string password)
    {
        User? foundUser = null;
        var errorMessage = "Invalid email or password";


        if (user.UserName is not null)
        {
            foundUser = await _userManager.FindByNameAsync(user.UserName);
            errorMessage = "Invalid username or password";
        }
        else if (_customIdentityOptions.User.RequireUniqueEmail)
        {
            var encryptedEmail = _emailEncryptionHelper.EncryptEmail(user.Email!);
            foundUser = await _userManager.FindByEmailAsync(encryptedEmail);
        }

        if (foundUser is not null && await _userManager.CheckPasswordAsync(foundUser, password))
        {
            var refreshTokenEntity = _refreshTokenHelper.GenerateRefreshTokenEntity(foundUser.Id);
            await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity!);

            return new TokensResult()
            {
                AccessToken = _accessTokenHelper.GenerateAccessToken(foundUser.Id),
                RefreshToken = _refreshTokenHelper.GenerateRefreshToken(foundUser.Id, refreshTokenEntity!.Id)
            };
        }

        // To prevent timing attacks, the password should be hashed even if a user does not exist.
        _userManager.PasswordHasher.HashPassword(null!, password);
        throw new LoginException(errorMessage);
    }
}
