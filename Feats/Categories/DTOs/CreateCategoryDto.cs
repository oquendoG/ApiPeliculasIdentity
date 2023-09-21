using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Feats.Categories.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(64, ErrorMessage = "Máximo número de caracteres es de 64")]
    public string Name { get; set; } = string.Empty;
}
