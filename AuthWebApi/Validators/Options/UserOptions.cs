namespace Validators.Options;
public class UserOptions
{
    public int MinUserNameLength { get; set; } = 3;

    public int MaxUserNameLength { get; set; } = 30;

    public string AllowedUserNameCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.";

    public bool RequireUniqueEmail { get; set; }
}