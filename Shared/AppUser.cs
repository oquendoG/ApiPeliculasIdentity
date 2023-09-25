using Microsoft.AspNetCore.Identity;

namespace ApiPeliculasIdentity.Shared;

public class AppUser : IdentityUser
{
    public required string Name { get; set; }
}
