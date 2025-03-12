namespace Midiot.BL.Models.UserProfile;

public class GetProfileModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorAuthEnabled { get; set; }
}
