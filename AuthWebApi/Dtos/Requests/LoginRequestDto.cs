namespace Dtos.Requests;
public class LoginRequestDto
{
    public required string UserNameOrEmail { get; set; }

    public required string Password { get; set; }
}