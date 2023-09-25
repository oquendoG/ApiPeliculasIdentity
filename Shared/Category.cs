using System.ComponentModel.DataAnnotations;

namespace ApiPeliculasIdentity.Shared;

public class Category
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreationDate { get; set; }
}
