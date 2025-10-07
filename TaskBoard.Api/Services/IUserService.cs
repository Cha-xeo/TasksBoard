using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services
{
    public interface IUserService
    {
        Task<List<UserDto>?> getAllUsersAsync();
        Task<UserDto?> getUserByIDAsync(int ID);
        Task<UserDto> CreateUserAsync(Users newUser);
        Task<UserDto?> UpdateUserAsync(int id, Users user);
        Task<bool> DeleteUserAsync(int ID);
        UserDto MapToDto(Users user);
    }
}
