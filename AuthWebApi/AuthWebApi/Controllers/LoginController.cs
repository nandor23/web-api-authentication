using Dtos.Requests;
using Helpers.Interfaces;
using Mappers;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;


namespace AuthWebApi.Controllers;

[Route("api/auth/login")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ICookieHelper _cookieHelper;
    private readonly ILogger<LoginController> _logger;
    private readonly UserMapper _userMapper;

    public LoginController(ILoginService loginService, ICookieHelper cookieHelper, ILogger<LoginController> logger)
    {
        _loginService = loginService;
        _cookieHelper = cookieHelper;
        _logger = logger;
        _userMapper = new UserMapper();
    }

    [HttpPost]
    public async Task<IActionResult> LoginUserAsync(LoginRequestDto loginRequestDto)
    {
        var user = _userMapper.LoginDtoToUser(loginRequestDto);
        var result = await _loginService.LoginUserAsync(user, loginRequestDto.Password);
        _cookieHelper.SetAccessTokenResponseCookie(result.AccessToken);
        _cookieHelper.SetRefreshTokenResponseCookie(result.RefreshToken);
        return Ok();
    }
}