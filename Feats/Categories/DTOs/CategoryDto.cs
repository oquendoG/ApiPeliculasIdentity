using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Feats.Categories.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(64, ErrorMessage = "Máximo número de caracteres es de 64")]
    public string Name { get; set; } = string.Empty;
}
