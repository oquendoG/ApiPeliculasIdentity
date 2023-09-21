using ApiPeliculas.Shared;

namespace ApiPeliculas.Feats.Users.DTOs;

/// <summary>
/// Respuesta del servidor al autenticarse
/// </summary>
public class UserLoginResponseDto
{
    public required User? User { get; set; }
    public required string Token { get; set; }

    public bool IsAuthenticated { get; set; } = false;
    public string? Message { get; set; }
}
