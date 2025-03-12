using Midiot.BL.Models.Auth;

namespace Midiot.BL.Interfaces.Auth;

public interface IAuthService
{
    Task<TokensInfoModel> SignInAsync(SignInModel model);
    Task<TokensInfoModel> RefreshTokensAsync(string refreshToken);
    Task<string> Send2FACodeAsync(SignInModel model);
    Task<TokensInfoModel> SignInWithCodeAsync(SignInWithCodeModel model);
    Task<bool> IsTwoFactorAuthEnabled(string email);
}
