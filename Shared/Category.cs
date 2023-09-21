using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Shared;

public class Category
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreationDate { get; set; }
}
