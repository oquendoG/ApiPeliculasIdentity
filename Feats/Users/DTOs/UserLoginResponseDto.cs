using ApiPeliculasIdentity.Shared;

namespace ApiPeliculasIdentity.Feats.Users.DTOs;

/// <summary>
/// Respuesta del servidor al autenticarse
/// </summary>
public class UserLoginResponseDto
{
    public required UserDataDto? User { get; set; }

    public bool IsAuthenticated { get; set; } = false;
    public string? Message { get; set; }
}
