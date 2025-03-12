using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Midiot.BL.Helpers;
using Midiot.BL.Interfaces.Email;
using Midiot.BL.Interfaces.User;
using Midiot.BL.Models.Auth;
using Midiot.BL.Models.UserAccount;
using Midiot.Common;
using Midiot.Common.Exceptions;
using Midiot.Common.Helpers;
using Midiot.Data;
using Midiot.Data.Entities;
using Midiot.Infrastructure.Entities;
using SendGrid.Helpers.Mail;

namespace Midiot.BL.Services.User;

public class UserAccountService : IUserAccountService
{
    private readonly AppDbContext _context;
    private readonly IUserExistenceService _userExistenceService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public UserAccountService(AppDbContext context, IUserExistenceService userExistenceService, IMapper mapper, IEmailService emailService)
    {
        _context = context;
        _userExistenceService = userExistenceService;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task SignUpAsync(SignUpModel signUpModel)
    {
        await _userExistenceService.EnsureUserDoesNotExist(signUpModel.Email);

        var code = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            IsUsed = false,
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = ConfirmationCodeMetadata.EmailConfirmation
        };

        var userEntity = _mapper.Map<UserEntity>(signUpModel);
        userEntity.PasswordHash = InformationHasher.HashText(signUpModel.Password);
        userEntity.ConfirmationCodes = new List<ConfirmationCodeEntity> { code };

        var emailMessage = MessageBuilder.BuildEmailMessage(userEntity.Email, userEntity.Name, code.Value);
        var message = MessageBuilder.CreateSendGridMessage(emailMessage, SendGridTemplate.SignUp);
        await _emailService.SendEmailAsync(message);

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
    }

    public async Task ConfirmEmailAsync(EmailConfirmationModel model)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).Where(u => u.Email == model.Email).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        if (user.IsEmailConfirmed)
        {
            throw new PermissionException("You have already confirmed your email address");
        }

        var userCode = user.ConfirmationCodes.FirstOrDefault(c => c.Value == model.Code);
        if (userCode == null
            || userCode.IsUsed
            || userCode.Expiration < DateTime.UtcNow
            || userCode.Metadata != ConfirmationCodeMetadata.EmailConfirmation)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        userCode.IsUsed = true;
        user.IsEmailConfirmed = true;
        await _context.SaveChangesAsync();
    }

    public async Task ResendEmailConfirmationCodeAsync(string email)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).Where(u => u.Email == email).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        if (user.IsEmailConfirmed)
        {
            throw new PermissionException("You have already confirmed your email address");
        }

        ConfirmationCodesInvalidator.InvalidatePreviousConfirmationCodes(user.ConfirmationCodes);

        var code = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            IsUsed = false,
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = ConfirmationCodeMetadata.EmailConfirmation
        };

        var emailMessage = MessageBuilder.BuildEmailMessage(user.Email, user.Name, code.Value);
        var message = MessageBuilder.CreateSendGridMessage(emailMessage, SendGridTemplate.SignUp);
        await _emailService.SendEmailAsync(message);

        user.ConfirmationCodes.Add(code);
        await _context.SaveChangesAsync();
    }

    public async Task RequestTwoFactorAuthChangeAsync(AuthTypeChangeModel model, Guid userId)
    {
        var userModel = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Email, u.Name })
            .FirstOrDefaultAsync();

        ValidationHelper.EnsureEntityFound(userModel);

        string code = CodeCreator.GenerateCode();

        var confirmationCode = new ConfirmationCodeEntity
        {
            Value = code,
            IsUsed = false,
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = model.Enable.Value ? ConfirmationCodeMetadata.Enable2FA : ConfirmationCodeMetadata.Disable2FA,
            UserId = userId,
        };

        var emailMessage = MessageBuilder.BuildEmailMessage(userModel.Email, userModel.Name, code);
        SendGridMessage message;
        if (model.Enable.Value)
        {
            message = MessageBuilder.CreateSendGridMessage(emailMessage, SendGridTemplate.TurnOn2FA);
        }
        else
        {
            message = MessageBuilder.CreateSendGridMessage(emailMessage, SendGridTemplate.TurnOff2FA);
        }

        await _emailService.SendEmailAsync(message);
        _context.ConfirmationCodes.Add(confirmationCode);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ConfirmTwoFactorAuthChangeAsync(ConfirmAuthTypeChangeModel model, Guid userId)
    {
        var confirmationCode = await _context.ConfirmationCodes
            .Include(c => c.User)
            .Where(c => c.UserId == userId && c.Value == model.Code)
            .FirstOrDefaultAsync();

        ValidationHelper.EnsureEntityFound(confirmationCode);
        if (confirmationCode.IsUsed
            || confirmationCode.Expiration < DateTime.UtcNow)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        var isTwoFactorAuthEnabled = confirmationCode.Metadata == ConfirmationCodeMetadata.Enable2FA;
        confirmationCode.User.IsTwoFactorAuthEnabled = isTwoFactorAuthEnabled;
        confirmationCode.IsUsed = true;

        await _context.SaveChangesAsync();
        return isTwoFactorAuthEnabled;
    }

    public async Task ChangePasswordAsync(ChangePasswordModel model, Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        if (InformationHasher.VerifyText(model.OldPassword, user.PasswordHash) == false)
        {
            throw new PermissionException("Invalid old password");
        }

        user.PasswordHash = InformationHasher.HashText(model.NewPassword);
        await _context.SaveChangesAsync();
    }

    public async Task RequestEmailChangingAsync(ChangeEmailModel model, Guid userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var confirmationCode = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = model.NewEmail,
            UserId = userId,
        };

        var emailMessage = MessageBuilder.BuildEmailMessage(user.Email, user.Name, confirmationCode.Value);
        var message = MessageBuilder.CreateSendGridMessage(emailMessage, SendGridTemplate.ChangeEmail);
        await _emailService.SendEmailAsync(message);

        _context.ConfirmationCodes.Add(confirmationCode);
        await _context.SaveChangesAsync();
    }

    public async Task ConfirmEmailChangingAsync(ConfirmChangeEmailModel model, Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.ConfirmationCodes)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var code = user.ConfirmationCodes.FirstOrDefault(cc => cc.Value == model.Code);
        if (code == null || code.IsUsed || code.Expiration < DateTime.UtcNow || code.Metadata != model.NewEmail)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        user.Email = model.NewEmail;
        code.IsUsed = true;
        await _context.SaveChangesAsync();
    }
}
