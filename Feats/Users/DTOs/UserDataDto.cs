using System.ComponentModel.DataAnnotations;

namespace ApiPeliculasIdentity.Feats.Users.DTOs;

public class UserDataDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
