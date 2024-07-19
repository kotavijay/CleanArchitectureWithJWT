using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DTOs;
using Application.Repositories;
using Domain.Entites;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public class UserService : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginDto loginUser)
    {
        var user = await FindUserByEmailAsync(loginUser.Email!);

        if (user == null)
            return new LoginResponseDto(false, "User not found!");

        var verifyUser = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);

        if (verifyUser)
        {
            string token = GenerateJWTToken(user);
            return new LoginResponseDto(true, "Login Success!", token);
        }
        else
        {
            return new LoginResponseDto(false, "Invalid Credentials!");
        }
    }

    public async Task<RegistrationResponseDto> RegisterUserAsync(RegisterUserDto registerUser)
    {
        var user = await FindUserByEmailAsync(registerUser.Email!);

        if (user != null)
            return new RegistrationResponseDto(false, "User Already Exists!");

        _context.Users.Add(new ApplicationUser()
        {
            Name = registerUser.Name,
            Email = registerUser.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password)
        });

        await _context.SaveChangesAsync();

        return new RegistrationResponseDto(true, "Registration Complete!");
    }

    #region private methods
    private async Task<ApplicationUser> FindUserByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    private string GenerateJWTToken(ApplicationUser user)
    {
        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
            new Claim(ClaimTypes.Name, user.Name!),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    #endregion //private methods
}
