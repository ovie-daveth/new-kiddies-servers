using ChatApp.Backend.DTOs;

namespace ChatApp.Backend.Services;

public interface IAuthService
{
    Task<AuthResponseDto> Register(RegisterDto registerDto);
    Task<AuthResponseDto> Login(LoginDto loginDto);
    Task<UserDto?> GetUserById(int userId);
    Task<List<UserDto>> SearchUsers(string query);
}

