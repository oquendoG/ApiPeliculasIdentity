using ApiPeliculasIdentity.Feats.Users.DTOs;
using ApiPeliculasIdentity.Shared;

namespace ApiPeliculasIdentity.Feats.Users.Repository;

public interface IUserRepository
{
    Task<AppUser?> GetUser(string id);
    Task<ICollection<AppUser>> GetUsers();
    Task<bool> IsUniqueUser(string username);
    Task<UserLoginResponseDto> Login(LoginUserDto loginUser);
    Task<UserDataDto> Register(RegisterUserDto registerUser);
}
