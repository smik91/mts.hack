using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.UserProfile;

public class UpdateProfileModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
}
