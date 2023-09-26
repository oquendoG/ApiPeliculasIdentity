using ApiPeliculasIdentity.Data;
using ApiPeliculasIdentity.Feats.Users.DTOs;
using ApiPeliculasIdentity.Shared;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculasIdentity.Feats.Users.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext context;
    private readonly UserManager<AppUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly SignInManager<AppUser> signInManager;
    private readonly ILogger<UserRepository> logger;

    public UserRepository(AppDbContext context, UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager,
        ILogger<UserRepository> logger)
    {
        this.context = context;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.signInManager = signInManager;
        this.logger = logger;
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

        await signInManager.SignInAsync(user, isPersistent: true);

        await userManager.AddToRoleAsync(user, "registrado");
        AppUser? registeredUser = await context.Users
            .FirstOrDefaultAsync(user => user.UserName == registerUser.UserName);

        return registeredUser!.Adapt<UserDataDto>();
    }

    public async Task<UserLoginResponseDto> Login(LoginUserDto loginUser)
    {
        try
        {
            AppUser? userDb = await context.Users
            .FirstOrDefaultAsync(user => user.UserName == loginUser.UserName);

            bool isValid = await userManager
                .CheckPasswordAsync(userDb!, loginUser.Password);

            UserLoginResponseDto response = new()
            {
                User = null,
                IsAuthenticated = false,
                Message = "Usuario no encontrado"
            };

            if (userDb is null || !isValid)
            {
                return response;
            }

            SignInResult result = await signInManager
                .PasswordSignInAsync(loginUser.UserName, loginUser.Password, true, true);

            if (result.IsLockedOut)
            {
                response.User = userDb.Adapt<UserDataDto>();
                response.IsAuthenticated = false;
                response.Message = "El usuario esta bloqueado, demasiados intentos, pruebe más tarde";

                return response;
            }

            if (!result.Succeeded)
            {
                response.User = userDb.Adapt<UserDataDto>();
                response.IsAuthenticated = false;
                response.Message = "Falló al autenticar, revise los campos";

                return response;
            }

            response.User = userDb.Adapt<UserDataDto>();
            response.IsAuthenticated = true;
            response.Message = "El usuario está autenticado en el sistema";

            return response;
        }
        catch (Exception ex)
        {
            UserLoginResponseDto response = new()
            {
                User = null,
                IsAuthenticated = false,
                Message = $"se ha producido una excepción al iniciar sesión: {ex.Message}",
            };

            logger.LogError(ex, "Se ha producido una excepción al autenticarse");
            return response;
        }
        
    }
}
