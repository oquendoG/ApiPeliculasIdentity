using ApiPeliculas.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Shared;

public class Movie
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [MaxLength(128, ErrorMessage = "Máximo número de caracteres es de 128")]
    public string Name { get; set; } = string.Empty;

    public string? Path { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [MaxLength(512, ErrorMessage = "Máximo número de caracteres es de 512")]
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; } = 0;

    public Pegi Pegi { get; set; }
    public DateTimeOffset CreationDate { get; set; }

    [ForeignKey("CategoryId")]
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}
