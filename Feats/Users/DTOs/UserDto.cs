﻿using System.ComponentModel.DataAnnotations;

namespace ApiPeliculasIdentity.Feats.Users.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTimeOffset CreationDate { get; set; }
}
