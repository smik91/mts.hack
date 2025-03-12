using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.Auth;

public class SignInWithCodeModel
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(100, ErrorMessage = "Max email length is 100 symbols")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Code is required")]
    [MaxLength(6, ErrorMessage = "Max code length is 6 symbols")]
    public string Code { get; set; }
}
