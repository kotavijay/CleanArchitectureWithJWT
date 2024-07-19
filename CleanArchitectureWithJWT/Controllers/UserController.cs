using Application.DTOs;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureWithJWT;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto login)
    {
        var result = await _userRepository.LoginUserAsync(login);

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegistrationResponseDto>> RegisterUser(RegisterUserDto registerUser)
    {
        var result = await _userRepository.RegisterUserAsync(registerUser);

        return Ok(result);
    }
}
