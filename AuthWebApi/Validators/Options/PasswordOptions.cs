namespace Validators.Options;
public class PasswordOptions
{
    public int MinLength { get; set; } = 8;

    public int MaxLength { get; set; } = 64;

    public int RequiredUniqueChars { get; set; } = 1;

    public bool RequireNonAlphanumeric { get; set; } = true;

    public bool RequireAnyLetter { get; set; } = false;

    public bool RequireLowercase { get; set; } = true;

    public bool RequireUppercase { get; set; } = true;

    public bool RequireDigit { get; set; } = true;
}