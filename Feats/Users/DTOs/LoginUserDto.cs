﻿using System.ComponentModel.DataAnnotations;

namespace ApiPeliculasIdentity.Feats.Users.DTOs;

public class LoginUserDto
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Password { get; set; } = string.Empty;
}
