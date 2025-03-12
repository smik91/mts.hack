using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.UserAccount;

public class ChangeEmailModel
{
    [Required(ErrorMessage = "New email is required")]
    public string NewEmail { get; set; }
}
