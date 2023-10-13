using Helpers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Environment;
using Shared.Settings;

namespace Helpers.Implementations;
public class CookieHelper : ICookieHelper
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AccessTokenSettings _accessTokenSettings;
    private readonly RefreshTokenSettings _refreshTokenSettings;
    private readonly SameSiteMode _sameSiteAttribute;


    public CookieHelper(IHttpContextAccessor httpContextAccessor, IOptions<AccessTokenSettings> accessTokenSettings, IOptions<RefreshTokenSettings> refreshTokenSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _accessTokenSettings = accessTokenSettings.Value;
        _refreshTokenSettings = refreshTokenSettings.Value;
        _sameSiteAttribute = EnvironmentProvider.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict;
    }

    public void SetAccessTokenResponseCookie(string accessToken)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieNames.AccessToken, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = _sameSiteAttribute,
            Expires = DateTime.UtcNow.AddMinutes(_accessTokenSettings.ExpirationInMinutes),
        });
    }

    public void SetRefreshTokenResponseCookie(string refreshToken)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieNames.RefreshToken, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = _sameSiteAttribute,
            Expires = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationInDays),
        });
    }
}