using ApiPeliculasIdentity.Data;
using ApiPeliculasIdentity.Feats.Users.DTOs;
using ApiPeliculasIdentity.Shared;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculasIdentity.Feats.Users.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext context;
    private readonly UserManager<AppUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly string PasswordJwt;
    public UserRepository(AppDbContext context,
        IConfiguration configuration, UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        this.context = context;
        this.userManager = userManager;
        this.roleManager = roleManager;
        PasswordJwt = configuration.GetValue<string>("ApiSettings:PasswordJwt")!;
    }

    public async Task<AppUser?> GetUser(string id)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<ICollection<AppUser>> GetUsers()
    {
        return await context.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();
    }

    public async Task<bool> IsUniqueUser(string username)
    {
        AppUser? userDb = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserName == username);

        return userDb == null;
    }

    public async Task<UserDataDto> Register(RegisterUserDto registerUser)
    {

        AppUser user = new()
        {
            UserName = registerUser.UserName,
            Name = registerUser.Name,
            Email = registerUser.UserName,
            NormalizedEmail = registerUser.UserName.ToUpper(),
        };

        IdentityResult result = await userManager
            .CreateAsync(user, registerUser.Password);

        if (!result.Succeeded)
        {
            return new UserDataDto();
        }

        if (result.Succeeded && !roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
            await roleManager.CreateAsync(new IdentityRole("registrado"));
        }

        await userManager.AddToRoleAsync(user, "registrado");
        AppUser? registeredUser = await context.Users
            .FirstOrDefaultAsync(user => user.UserName == registerUser.UserName);

        return registeredUser!.Adapt<UserDataDto>();
    }

    public async Task<UserLoginResponseDto> Login(LoginUserDto loginUser)
    {
        AppUser? userDb = await context.Users
            .FirstOrDefaultAsync(user => user.UserName == loginUser.UserName);

        bool isValid = await userManager
            .CheckPasswordAsync(userDb!, loginUser.Password);

        UserLoginResponseDto response = new()
        {
            Token = string.Empty,
            User = null,
            IsAuthenticated = false,
            Message = "Usuario no encontrado"
        };

        if (userDb is null || !isValid)
        {
            return response;
        }

        IList<string> roles = await userManager.GetRolesAsync(userDb);

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
                new Claim(ClaimTypes.Name, userDb.UserName!),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = jwtHandler.CreateToken(tokenDescriptor);

        response.Token = jwtHandler.WriteToken(token);
        response.User = userDb.Adapt<UserDataDto>();
        response.IsAuthenticated = true;
        response.Message = "El usuario esta autenticado en el sistema";

        return response;
    }
}
