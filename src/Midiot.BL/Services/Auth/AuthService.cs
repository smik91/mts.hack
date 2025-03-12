using Microsoft.EntityFrameworkCore;
using Midiot.BL.Helpers;
using Midiot.BL.Interfaces.Auth;
using Midiot.BL.Interfaces.Email;
using Midiot.BL.Models.Auth;
using Midiot.Common;
using Midiot.Common.Exceptions;
using Midiot.Common.Helpers;
using Midiot.Data;
using Midiot.Data.Entities;
using Midiot.Infrastructure.Entities;

namespace Midiot.BL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private const int REFRESH_TOKEN_EXPIRATION_DAYS = 30;
    private readonly IEmailService _emailService;

    public AuthService(ITokenService tokenService, AppDbContext context, IEmailService emailService)
    {
        _tokenService = tokenService;
        _context = context;
        _emailService = emailService;
    }

    public async Task<TokensInfoModel> SignInAsync(SignInModel signInModel)
    {
        var user = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == signInModel.Email);
        ValidationHelper.EnsureEntityFound(user);
        ValidationHelper.ValidateSignInData(user.PasswordHash, signInModel.Password);

        if (user.IsEmailConfirmed == false)
        {
            throw new PermissionException("You must confirm your email to authenticate.");
        }

        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task<TokensInfoModel> RefreshTokensAsync(string refreshToken)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .Where(u => u.RefreshTokens.Any(rt => rt.Value == refreshToken))
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Value == refreshToken);
        ValidationHelper.ValidateRefreshToken(userRefreshToken.Expiration);

        userRefreshToken.Expiration = DateTime.UtcNow;
        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task<string> Send2FACodeAsync(SignInModel model)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).FirstOrDefaultAsync(u => u.Email == model.Email);
        ValidationHelper.EnsureEntityFound(user);
        ValidationHelper.ValidateSignInData(user.PasswordHash, model.Password);
        if (user.IsEmailConfirmed == false)
        {
            throw new PermissionException("You must confirm your email to authenticate.");
        }

        ConfirmationCodesInvalidator.InvalidatePreviousConfirmationCodes(user.ConfirmationCodes);

        var code = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            IsUsed = false,
            Metadata = ConfirmationCodeMetadata.TwoFactorAuth,
            Expiration = DateTime.UtcNow.AddMinutes(5)
        };

        var emailMessage = MessageBuilder.BuildEmailMessage(user.Email, user.Name, code.Value);
        var message = MessageBuilder.CreateSendGridMessage(emailMessage, SendGridTemplate.SignIn);
        await _emailService.SendEmailAsync(message);

        user.ConfirmationCodes.Add(code);
        await _context.SaveChangesAsync();
        return user.Email;
    }

    public async Task<TokensInfoModel> SignInWithCodeAsync(SignInWithCodeModel model)
    {
        var user = await _context.Users
            .Include(u => u.ConfirmationCodes)
            .Include(u => u.RefreshTokens)
            .Where(u => u.Email == model.Email)
            .FirstOrDefaultAsync();

        ValidationHelper.ValidateSignInData(user.PasswordHash, model.Password);

        if (user.IsEmailConfirmed == false)
        {
            throw new PermissionException("You must confirm your email.");
        }

        var userCode = user.ConfirmationCodes.FirstOrDefault(c => c.Value == model.Code);
        if (userCode == null
            || userCode.IsUsed
            || userCode.Expiration < DateTime.UtcNow
            || userCode.Metadata != ConfirmationCodeMetadata.TwoFactorAuth)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        userCode.IsUsed = true;
        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task<bool> IsTwoFactorAuthEnabled(string email)
    {
        var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        var is2FAEnabled = user.IsTwoFactorAuthEnabled;
        return is2FAEnabled;
    }

    private async Task<TokensInfoModel> GenerateAndSaveTokensAsync(UserEntity user)
    {
        var accessTokenInfo = _tokenService.GenerateAccessToken(user.Id);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRATION_DAYS);

        var refreshTokenEntity = new RefreshTokenEntity
        {
            Value = refreshTokenValue,
            Expiration = refreshTokenExpiryTime
        };

        user.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var tokensInfo = new TokensInfoModel
        {
            AccessToken = accessTokenInfo.Value,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiration = accessTokenInfo.Expiration,
            RefreshTokenExpiration = refreshTokenExpiryTime
        };

        return tokensInfo;
    }
}
