using Midiot.BL.Models.Auth;

namespace Midiot.BL.Interfaces.Auth;

public interface ITokenService
{
    AccessTokenInfo GenerateAccessToken(Guid userId);
    string GenerateRefreshToken();
}
