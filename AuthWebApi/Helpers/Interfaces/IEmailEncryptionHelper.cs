namespace Helpers.Interfaces;
public interface IEmailEncryptionHelper
{
    string EncryptEmail(string email);

    string DecryptEmail(string email);
}