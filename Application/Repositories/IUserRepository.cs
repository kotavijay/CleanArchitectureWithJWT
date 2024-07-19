using Application.DTOs;

namespace Application.Repositories;

public interface IUserRepository
{
    Task<RegistrationResponseDto> RegisterUserAsync(RegisterUserDto registerUser);

    Task<LoginResponseDto> LoginUserAsync(LoginDto loginUser);
}
