using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.Auth;

public class SignUpModel
{
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, ErrorMessage = "Email max length is 100 symbols")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name max length is 50 symbols")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
