using System.Security.Cryptography;
using System.Text;
using Helpers.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Helpers.Implementations;
public class EmailEncryptionHelper : IEmailEncryptionHelper
{
    private readonly EmailEncryptionSettings _emailEncryptionSettings;

    public EmailEncryptionHelper(IOptions<EmailEncryptionSettings> emailEncryptionSettings)
    {
        _emailEncryptionSettings = emailEncryptionSettings.Value;
    }

    public string EncryptEmail(string email)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_emailEncryptionSettings.EncryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(_emailEncryptionSettings.InitializationVector);
        var encryption = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new();

        using (CryptoStream cryptoStream = new(memoryStream, encryption, CryptoStreamMode.Write))
        {
            using StreamWriter encryptWriter = new(cryptoStream);
            encryptWriter.Write(email);
        }

        var encryptedBytes = memoryStream.ToArray();

        return Convert.ToBase64String(encryptedBytes);
    }

    public string DecryptEmail(string email)
    {
        var buffer = Convert.FromBase64String(email);
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_emailEncryptionSettings.EncryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(_emailEncryptionSettings.InitializationVector);

        var decryption = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new(buffer);
        using CryptoStream cryptoStream = new(memoryStream, decryption, CryptoStreamMode.Read);
        using StreamReader decryptReader = new(cryptoStream);
        return decryptReader.ReadToEnd();
    }
}