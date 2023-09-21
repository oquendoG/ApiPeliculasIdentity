using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Shared;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(128)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [MaxLength(128)]
    public string Role { get; set; } = string.Empty;

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
}
