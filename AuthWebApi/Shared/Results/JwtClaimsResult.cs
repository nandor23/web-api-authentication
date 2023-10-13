namespace Shared.Results;
public class JwtClaimsResult
{
    public Guid Jti { get; set; }

    public Guid UserId { get; set; }
}