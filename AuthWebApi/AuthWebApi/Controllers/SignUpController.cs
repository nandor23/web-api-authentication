using Dtos.Requests;
using Helpers.Interfaces;
using Mappers;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AuthWebApi.Controllers;

[Route("/api/auth/signup")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly ISignUpService _signUpService;
    private readonly ILogger<SignUpController> _logger;
    private readonly UserMapper _userMapper;

    public SignUpController(ISignUpService signUpService, ICookieHelper cookieHelper, ILogger<SignUpController> logger)
    {
        _signUpService = signUpService;
        _logger = logger;
        _userMapper = new UserMapper();
    }

    [HttpPost]
    public async Task<IActionResult> SignUpUserAsync(SignUpRequestDto signUpRequestDto)
    {
        var user = _userMapper.SignUpDtoToUser(signUpRequestDto);
        await _signUpService.SignUpUserAsync(user, signUpRequestDto.Password);
        return Ok();
    }
}