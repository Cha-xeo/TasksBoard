using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Mappers;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Services
{
    public class UserService : IUserService
    {
        private readonly TasksContext _tasksContext;

        // Map expansion keys (from query or service) to EF Core navigation expressions
        private static readonly Dictionary<string, Expression<Func<Users, object>>> ExpansionMap
            = new()
                {
                    { "tasks", u => u.Tasks }
                };

        public UserService(TasksContext tasksContext)
        {
            _tasksContext = tasksContext;
        }

        public async Task<List<UserDto>?> GetAllUsers(List<string>? expands = null)
        {

            IQueryable<Users> query = _tasksContext.Users.AsQueryable();

            if (expands is not null)
            {
                foreach (string? key in expands)
                {
                    if (ExpansionMap.TryGetValue(key, out var expr))
                    {
                        query = query.Include(expr);
                    }
                }
            }


            List<Users> users = await query.ToListAsync();
            return users.Count == 0 ? null : users.Select(u => UserMapper.ToDto(u, expands)).ToList();
        }

        public async Task<UserDto?> GetById(int ID, List<string>? expands = null)
        {
            IQueryable<Users> query = _tasksContext.Users.IgnoreQueryFilters().AsQueryable();

            if (expands is not null)
            {
                foreach (string? key in expands)
                {
                    if (ExpansionMap.TryGetValue(key, out var expr))
                    {
                        query = query.Include(expr);
                    }
                }
            }

            Users? user = await query.FirstOrDefaultAsync(u => u.ID == ID);

            return user is null ? null : UserMapper.ToDto(user, expands);
        }

        public async Task<UserDto> Create(Users newUser, List<string>? expands = null)
        {
            try
            {
                _tasksContext.Add(newUser);
                await _tasksContext.SaveChangesAsync();
                return UserMapper.ToDto(newUser, expands);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                throw;
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine($"Validation error: {argEx.Message}");
                throw;
            }
        }

        public async Task<UserDto?> Update(int ID, UserUpdateDto userUpdate, List<string>? expands = null)
        {
            Users? existingUser = await _tasksContext.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.ID == ID);

            if (existingUser is null) return null;
            if (userUpdate.ID != existingUser.ID)
                throw new ApiException("Mismatched updateUser ID", 400);

            // Handle navigation values and validate TasksIds
            await ValidateTasksAsync(existingUser, userUpdate.TasksIDS);

            UserMapper.FromUserUpdateDto(existingUser, userUpdate);

            await _tasksContext.SaveChangesAsync();
            return UserMapper.ToDto(existingUser, expands);
        }
        public async Task<UserDto?> RestoreSoftDeleted(int ID)
        {
            Users? user = await _tasksContext.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.ID == ID);
            if (user is null) return null;

            user.DeletedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;
            await _tasksContext.SaveChangesAsync();
            return UserMapper.ToDto(user);
        }

        public async Task<bool> SoftDelete(int ID)
        {
            Users? user = await _tasksContext.Users.FirstOrDefaultAsync(u => u.ID == ID);
            if (user is null) return false;
            user.UpdatedAt = DateTime.UtcNow;
            user.DeletedAt = DateTime.UtcNow;

            if (user.Tasks is not null)
            {
                foreach (var task in user.Tasks)
                {
                    task.UpdatedAt = DateTime.UtcNow;
                    task.DeletedAt = DateTime.UtcNow;
                }
            }
            await _tasksContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int ID)
        {
            Users? toDelete = await _tasksContext.Users.FindAsync(ID);
            if (toDelete is null) return false;
            _tasksContext.Users.Remove(toDelete);
            await _tasksContext.SaveChangesAsync();
            return true;
        }

        public async Task ValidateTasksAsync(Users newUser, List<int>? requestedIds)
        {
            List<Tasks>? existingRequestedTasks = null;
            if (requestedIds is not null)
            {
                existingRequestedTasks = await _tasksContext.Tasks
                    .Where(t => requestedIds.Contains(t.ID))
                    .ToListAsync();
                if (existingRequestedTasks.Count != requestedIds.Count)
                {
                    throw new ApiException("Some assigned tasks do not exist.", 400);
                }
            }
            Helper.ColletionHelpers.SyncCollection(newUser.Tasks, existingRequestedTasks, t => t.ID);
        }
    }
}
