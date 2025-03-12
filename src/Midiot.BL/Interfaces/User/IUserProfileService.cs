using Midiot.BL.Models.UserProfile;

namespace Midiot.BL.Interfaces.User;

public interface IUserProfileService
{
    Task<GetProfileModel> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(UpdateProfileModel model, Guid userId);
}
