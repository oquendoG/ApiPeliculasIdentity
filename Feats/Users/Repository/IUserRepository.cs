using ApiPeliculas.Feats.Users.DTOs;
using ApiPeliculas.Shared;

namespace ApiPeliculas.Feats.Users.Repository;

public interface IUserRepository
{
    Task<User?> GetUser(Guid id);
    Task<ICollection<User>> GetUsers();
    Task<bool> IsUniqueUser(string username);
    Task<UserLoginResponseDto> Login(LoginUserDto loginUser);
    Task<User> Register(RegisterUserDto registerUser);
}
