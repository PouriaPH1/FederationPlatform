using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(CreateUserDto dto);
    Task<AuthResultDto> LoginAsync(LoginDto dto);
    Task LogoutAsync();
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
}
