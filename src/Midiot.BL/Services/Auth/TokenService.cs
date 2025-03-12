using Microsoft.Extensions.Options;
using Midiot.Common;
using Midiot.Common.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Midiot.BL.Models.Auth;
using Midiot.BL.Interfaces.Auth;

namespace Midiot.BL.Services.Auth;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private const int EXPIRATION_SECONDS = 3600;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessTokenInfo GenerateAccessToken(Guid userId)
    {
        var claims = new List<Claim>();
        var userIdClaim = new Claim(CustomClaimTypes.UserId, userId.ToString());
        claims.Add(userIdClaim);

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddSeconds(EXPIRATION_SECONDS);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: expiration,
            issuer: _options.Issuer
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        var tokenInfo = new AccessTokenInfo
        {
            Value = tokenValue,
            Expiration = expiration
        };
        return tokenInfo;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
