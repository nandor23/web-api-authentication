namespace Shared.Settings;

public class EmailEncryptionSettings
{
    public required string EncryptionKey { get; set; }
    public required string InitializationVector { get; set; }
}