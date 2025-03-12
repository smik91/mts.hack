﻿using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.Auth;

public class SignInModel
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
