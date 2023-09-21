using ApiPeliculas.Data;
using ApiPeliculas.Feats.Users.DTOs;
using ApiPeliculas.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculas.Feats.Users.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext context;
    private readonly IPasswordHasher<User> passwordHasher;
    private readonly string? PasswordJwt;
    public UserRepository(AppDbContext context, 
        IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        this.context = context;
        this.passwordHasher = passwordHasher;
        PasswordJwt = configuration.GetValue<string>("ApiSettings:PasswordJwt");
    }

    public async Task<User?> GetUser(Guid id)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<ICollection<User>> GetUsers()
    {
        return await context.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();
    }

    public async Task<bool> IsUniqueUser(string username)
    {
        User? userDb = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserName == username);

        return userDb == null;
    }

    public async Task<User> Register(RegisterUserDto registerUser)
    {

        User user = new()
        {
            UserName = registerUser.UserName,
            Name = registerUser.Name,
            Role = registerUser.Role,
        };

        user.Password = passwordHasher.HashPassword(user, registerUser.Password);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<UserLoginResponseDto> Login(LoginUserDto loginUser)
    {
        User? userDb = await context.Users
            .FirstOrDefaultAsync(user => user.UserName == loginUser.UserName);

        UserLoginResponseDto response = new()
        {
            Token = string.Empty,
            User = null,
            IsAuthenticated = false,
            Message = "Usuario no encontrado"
        };

        if (userDb is null)
        {
            return response;
        }

        PasswordVerificationResult result = passwordHasher
            .VerifyHashedPassword(userDb, userDb.Password, loginUser.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            response.Message = "Credenciales de usuario incorrectas, verifique por favor";
            return response;
        }

        JwtSecurityTokenHandler jwtHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(PasswordJwt!);
        if (key.Length == 0)
        {
            response.IsAuthenticated = false;
            response.Message = "No se pudo hallar la clave del token";
            return response;
        }

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userDb.UserName),
                new Claim(ClaimTypes.Role, userDb.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = jwtHandler.CreateToken(tokenDescriptor);

        response.Token = jwtHandler.WriteToken(token);
        response.User = userDb;
        response.IsAuthenticated = true;
        response.Message = "El usuario esta autenticado en el sistema";

        return response;
    }
}
