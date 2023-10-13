namespace Helpers.Interfaces;
public interface IAccessTokenHelper
{
    string GenerateAccessToken(Guid userId);
}