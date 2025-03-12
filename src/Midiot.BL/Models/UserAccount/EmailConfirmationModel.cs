using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.UserAccount;

public class EmailConfirmationModel
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(100, ErrorMessage = "Email max length is 100 symbols")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Code is required")]
    [MaxLength(6, ErrorMessage = "Code max length is 6 symbols")]
    public string Code { get; set; }
}
