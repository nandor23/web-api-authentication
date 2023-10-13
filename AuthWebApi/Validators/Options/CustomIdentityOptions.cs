using Microsoft.AspNetCore.Identity;

namespace Validators.Options;
public class CustomIdentityOptions : IdentityOptions
{
    public new UserOptions User { get; set; } = new();

    public new PasswordOptions Password { get; set; } = new();
}