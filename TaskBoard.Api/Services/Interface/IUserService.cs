using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services.Interface
{
    public interface IUserService
    {
        Task<List<UserDto>?> GetAllUsers(List<string>? expands = null);
        Task<UserDto?> GetById(int ID, List<string>? expands = null);
        Task<UserDto> Create(Users newUser, List<string>? expands = null);
        //Task<UserDto?> Update(int ID, UserUpdateDto userUpdate, List<string>? expands = null);
        Task<UserDto?> Update(int ID, UserUpdateDto userUpdate, List<string>? expands = null);
        Task<bool> Delete(int ID);
        Task<bool> SoftDelete(int ID);
        Task<UserDto?> RestoreSoftDeleted(int ID);

        Task ValidateTasksAsync(Users newUser, List<int>? requestedIds);
    }
}
