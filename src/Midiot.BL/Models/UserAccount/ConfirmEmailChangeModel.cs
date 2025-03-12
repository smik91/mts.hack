using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.UserAccount;

public class ConfirmChangeEmailModel
{

    [Required(ErrorMessage = "New email is required")]
    public string NewEmail { get; set; }

    [Required(ErrorMessage = "Code is required")]
    [MaxLength(6, ErrorMessage = "Code max length is 6 symbols")]
    public string Code { get; set; }
}
