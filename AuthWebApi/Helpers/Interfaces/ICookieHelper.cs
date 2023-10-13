namespace Helpers.Interfaces;
public interface ICookieHelper
{
    void SetAccessTokenResponseCookie(string accessToken);

    void SetRefreshTokenResponseCookie(string refreshToken);
}