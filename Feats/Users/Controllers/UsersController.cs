using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using ApiPeliculasIdentity.Feats.Users.DTOs;
using ApiPeliculasIdentity.Feats.Users.Repository;
using ApiPeliculasIdentity.Shared;

namespace ApiPeliculasIdentity.Feats.Users.Controllers;

[Authorize]
[ApiController]
[Route("api/usuarios")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public ApiReponse ApiResponse { get; }

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        ApiResponse = new();
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsers()
    {
        ICollection<AppUser> usersDb =
            await userRepository.GetUsers();
        if (usersDb.Count == 0)
        {
            return NotFound("No hay usuarios que mostrar");
        }

        ICollection<UserDto> users =
            usersDb.Adapt<ICollection<UserDto>>();

        return Ok(users);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("{id}", Name = "usuario")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUser(string id)
    {
        AppUser? userDb =
            await userRepository.GetUser(id);
        if (userDb is null)
        {
            return NotFound(id);
        }

        UserDto user =
            userDb.Adapt<UserDto>();

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("registro")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto userToRegister)
    {
        if (!ModelState.IsValid || userToRegister is null)
        {
            ModelState.AddModelError(string.Empty, "Revise los datos de registro");
            return BadRequest(ModelState);
        }

        bool isUniqueUser = await userRepository.IsUniqueUser(userToRegister.UserName);
        if (!isUniqueUser)
        {
            ApiResponse.StatusCode = HttpStatusCode.BadRequest;
            ApiResponse.ErrorMessages.Add("El nombre de usuario ya existe");
            ApiResponse.IsSuccess = false;
            return BadRequest(ApiResponse);
        }

        UserDataDto user = await userRepository.Register(userToRegister);
        if (user is null)
        {
            ApiResponse.StatusCode = HttpStatusCode.BadRequest;
            ApiResponse.ErrorMessages.Add("Error en el registro, revise los campos");
            ApiResponse.IsSuccess = false;
            return BadRequest(ApiResponse);
        }

        ApiResponse.StatusCode = HttpStatusCode.OK;
        ApiResponse.IsSuccess = true;

        return Ok(ApiResponse);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto userToLogin)
    {
        if (!ModelState.IsValid || userToLogin is null)
        {
            ModelState.AddModelError(string.Empty, "Revise los datos de inicio de sesión");
            return BadRequest(ModelState);
        }

        UserLoginResponseDto response = await userRepository.Login(userToLogin);
        if (response.User is null)
        {
            ApiResponse.StatusCode = HttpStatusCode.BadRequest;
            ApiResponse.ErrorMessages.Add("Error en el inicio de sesión, revise los campos");
            ApiResponse.IsSuccess = false;
            return BadRequest(ApiResponse);
        }

        ApiResponse.StatusCode = HttpStatusCode.OK;
        ApiResponse.IsSuccess = true;
        ApiResponse.Result = response;
        return Ok(ApiResponse);
    }
}
