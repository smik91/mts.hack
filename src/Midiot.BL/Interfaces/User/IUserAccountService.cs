using Midiot.BL.Models.Auth;
using Midiot.BL.Models.UserAccount;

namespace Midiot.BL.Interfaces.User;

public interface IUserAccountService
{
    Task SignUpAsync(SignUpModel signUpModel);
    Task ConfirmEmailAsync(EmailConfirmationModel model);
    Task ResendEmailConfirmationCodeAsync(string email);
    Task RequestTwoFactorAuthChangeAsync(AuthTypeChangeModel model, Guid userId);
    Task<bool> ConfirmTwoFactorAuthChangeAsync(ConfirmAuthTypeChangeModel model, Guid userId);
    Task ChangePasswordAsync(ChangePasswordModel model, Guid userId);
    Task RequestEmailChangingAsync(ChangeEmailModel model, Guid userId);
    Task ConfirmEmailChangingAsync(ConfirmChangeEmailModel model, Guid userId);
}
