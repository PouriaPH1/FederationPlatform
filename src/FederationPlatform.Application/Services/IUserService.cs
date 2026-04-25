using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> BanUserAsync(int id);
    Task<bool> ActivateUserAsync(int id);
    Task<bool> PromoteToRepresentativeAsync(int id);
    Task<bool> DeleteUserAsync(int id);
    Task<int> GetTotalCountAsync();
}
