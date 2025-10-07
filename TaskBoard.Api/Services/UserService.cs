using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services
{
    public class UserService : IUserService
    {
        private readonly TasksContext _tasksContext;
        public UserService(TasksContext tasksContext)
        {
            _tasksContext = tasksContext;
        }

        public UserDto MapToDto(Users user)
        {
            return new UserDto
            {
                ID = user.ID,
                Age = user.Age,
                UserName = user.UserName,
                BirthDate = user.BirthDate
            };
        }
        public async Task<UserDto> CreateUserAsync(Users newUser)
        {
            try
            {
                _tasksContext.Add(newUser);
                await _tasksContext.SaveChangesAsync();
                return MapToDto(newUser);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int ID)
        {
            Users? toDelete = await _tasksContext.Users.FindAsync(ID);
            if (toDelete is null) return false;
            _tasksContext.Users.Remove(toDelete);
            await _tasksContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>?> getAllUsersAsync()
        {
            List<Users> users = await _tasksContext.Users.ToListAsync();
            //List<Users> users = await _tasksContext.Users.Include(_ => _.Tasks).ToListAsync();
            
            return users.Count == 0 ? null : users.Select(u => MapToDto(u)).ToList();
        }

        public async Task<UserDto?> getUserByIDAsync(int ID)
        {
            var user = await _tasksContext.Users.FindAsync(ID);
            Console.WriteLine(user);
            return user is null ? null : MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int ID, Users user)
        {
            var existingUser = await _tasksContext.Users.FindAsync(ID);
            if (existingUser is null) return null;
            if (user.ID != existingUser.ID) 
                throw new ApiException("Mismatched updateUser ID", 400);

            _tasksContext.Users.Entry(existingUser).CurrentValues.SetValues(user);

            await _tasksContext.SaveChangesAsync();
            return existingUser is null ? null : MapToDto(existingUser);
        }
    }
}
