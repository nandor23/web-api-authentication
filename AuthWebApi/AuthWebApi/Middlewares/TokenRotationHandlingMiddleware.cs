using Helpers.Interfaces;
using Services.Interfaces;
using Shared.Constants;
using Shared.Exceptions.AuthExceptions;

namespace AuthWebApi.Middlewares;

public class TokenRotationHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRotationHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenRotationService tokenRotationService, ICookieHelper cookieHelper)
    {
        try
        {
            var accessToken = context.Request.Cookies[CookieNames.AccessToken];
            var refreshToken = context.Request.Cookies[CookieNames.RefreshToken];

            if (accessToken is null || refreshToken is null)
            {
                var tokens = await tokenRotationService.RotateTokenAsync(refreshToken);

                context.Request.Headers[CookieNames.AccessToken] = tokens.AccessToken;

                cookieHelper.SetAccessTokenResponseCookie(tokens.AccessToken);
                cookieHelper.SetRefreshTokenResponseCookie(tokens.RefreshToken);
            }

            await _next(context);
        }
        catch (TokenRotationException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
        }
    }
}