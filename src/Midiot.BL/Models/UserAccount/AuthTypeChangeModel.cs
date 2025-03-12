using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.UserAccount;

public class AuthTypeChangeModel
{
    [Required(ErrorMessage = "Enable is required")]
    public bool? Enable { get; set; }
}
