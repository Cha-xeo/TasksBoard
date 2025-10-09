using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services
{
    public interface IUserService
    {
        Task<List<UserDto>?> GetAllUsers(List<string>? expands = null);
        Task<UserDto?> GetById(int ID, List<string>? expands = null);
        Task<UserDto> Create(Users newUser, List<string>? expands = null);
        Task<UserDto?> Update(int ID, Users user, List<string>? expands = null);
        Task<bool> Delete(int ID);
        Task ValidateTasksAsync(Users newUser);
        UserDto MapToDto(Users user, List<string>? expands = null);
    }
}
