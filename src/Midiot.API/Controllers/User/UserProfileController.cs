using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midiot.BL.Interfaces.User;
using Midiot.BL.Models.UserProfile;

namespace Midiot.API.Controllers.User;

[Route("profile")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ICurrentUserService _currentUserService;

    public UserProfileController(ICurrentUserService currentUserService, IUserProfileService userProfileService)
    {
        _currentUserService = currentUserService;
        _userProfileService = userProfileService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = _currentUserService.GetUserId();
        var profile = await _userProfileService.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userProfileService.UpdateProfileAsync(model, userId);
        return Ok();
    }
}
