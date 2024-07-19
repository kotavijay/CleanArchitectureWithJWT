namespace Application.DTOs;

public record LoginResponseDto(bool Flag, string Message = null!, string Token = null!);