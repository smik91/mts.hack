using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midiot.BL.Interfaces.User;
using Midiot.BL.Models.Auth;
using Midiot.BL.Models.UserAccount;

namespace Midiot.API.Controllers.User;

[Route("account")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly IUserAccountService _userAccountSerivce;
    private readonly ICurrentUserService _currentUserService;

    public UserAccountController(IUserAccountService userSerivce, ICurrentUserService currentUserService)
    {
        _userAccountSerivce = userSerivce;
        _currentUserService = currentUserService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(SignUpModel signUpModel)
    {
        await _userAccountSerivce.SignUpAsync(signUpModel);
        return Created();
    }

    [HttpPost("email/confirmation")]
    public async Task<IActionResult> ConfirmEmail(EmailConfirmationModel emailConfirmationModel)
    {
        await _userAccountSerivce.ConfirmEmailAsync(emailConfirmationModel);
        return Ok();
    }

    [HttpPost("email/confirmation/code")]
    public async Task<IActionResult> ResendEmailConfirmationCode(string email)
    {
        await _userAccountSerivce.ResendEmailConfirmationCodeAsync(email);
        return Ok();
    }

    [HttpPut("2fa")]
    [Authorize]
    public async Task<IActionResult> Request2FaChange(AuthTypeChangeModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userAccountSerivce.RequestTwoFactorAuthChangeAsync(model, userId);
        return Created();
    }

    [HttpPut("2fa/confirmation")]
    [Authorize]
    public async Task<IActionResult> Request2FaConfirmation(ConfirmAuthTypeChangeModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userAccountSerivce.ConfirmTwoFactorAuthChangeAsync(model, userId);
        return Ok();
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(ChangePasswordModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userAccountSerivce.ChangePasswordAsync(model, userId);
        return Ok();
    }

    [HttpPut("email-changing")]
    [Authorize]
    public async Task<IActionResult> UpdateEmail(ChangeEmailModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userAccountSerivce.RequestEmailChangingAsync(model, userId);
        return Created();
    }

    [HttpPut("email-changing/confirmation")]
    [Authorize]
    public async Task<IActionResult> ConfirmEmailChanging(ConfirmChangeEmailModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userAccountSerivce.ConfirmEmailChangingAsync(model, userId);
        return Ok();
    }
}
