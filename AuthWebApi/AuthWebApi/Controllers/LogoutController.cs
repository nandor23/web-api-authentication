using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Constants;

namespace AuthWebApi.Controllers;
[Route("api/auth/logout")]
[ApiController]
public class LogoutController : ControllerBase
{
    private readonly ILogoutService _logoutService;
    private readonly ILogger<LoginController> _logger;

    public LogoutController(ILogoutService logoutService, ILogger<LoginController> logger)
    {
        _logoutService = logoutService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> LogoutUserAsync()
    {
        var refreshToken = HttpContext.Request.Cookies[CookieNames.RefreshToken];

        await _logoutService.LogoutUserAsync(refreshToken);

        HttpContext.Response.Cookies.Delete(CookieNames.AccessToken);
        HttpContext.Response.Cookies.Delete(CookieNames.RefreshToken);
        return Ok();
    }
}