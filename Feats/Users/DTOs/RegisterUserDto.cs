using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Feats.Users.DTOs;

public class RegisterUserDto
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
