using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Models;

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
            List<Users> users = await _tasksContext.Users.ToListAsync();
            return users.Count == 0 ? null : users.Select(u => MapToDto(u, expands)).ToList();
        }

        public async Task<UserDto?> GetById(int ID, List<string>? expands = null)
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

            Users? user = await query.FirstOrDefaultAsync(u => u.ID == ID);

            return user is null ? null : MapToDto(user, expands);
        }

        public async Task<UserDto> Create(Users newUser, List<string>? expands = null)
        {
            try
            {
                await ValidateTasksAsync(newUser);

                _tasksContext.Add(newUser);
                await _tasksContext.SaveChangesAsync();
                return MapToDto(newUser, expands);
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

        public async Task<UserDto?> Update(int ID, Users user, List<string>? expands = null)
        {
            var existingUser = await _tasksContext.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.ID == ID);
            if (existingUser is null) return null;
            if (user.ID != existingUser.ID)
                throw new ApiException("Mismatched updateUser ID", 400);

            await ValidateTasksAsync(user);

            // handle scalar values
            _tasksContext.Users.Entry(existingUser).CurrentValues.SetValues(user);
            
            Helper.ColletionHelpers.SyncCollection(existingUser.Tasks, user.Tasks);

            await _tasksContext.SaveChangesAsync();
            return existingUser is null ? null : MapToDto(existingUser, expands);
        }

        public async Task<bool> Delete(int ID)
        {
            Users? toDelete = await _tasksContext.Users.FindAsync(ID);
            if (toDelete is null) return false;
            _tasksContext.Users.Remove(toDelete);
            await _tasksContext.SaveChangesAsync();
            return true;
        }

        public async Task ValidateTasksAsync(Users newUser)
        {
            if (newUser.Tasks is null || !newUser.Tasks.Any()) return;
            var taskIds = newUser.Tasks.Select(t => t.ID).ToList();

            var existingTasks = await _tasksContext.Tasks
                .Where(t => taskIds.Contains(t.ID))
                .ToListAsync();


            if (existingTasks.Count != taskIds.Count)
            {
                throw new ArgumentException("Some assigned tasks do not exist.");
            }

            newUser.Tasks = existingTasks;
        }

        public UserDto MapToDto(Users user, List<string>? expands = null)
        {
            // Default
            UserDto dto = new UserDto
            {
                ID = user.ID,
                Age = user.Age,
                UserName = user.UserName,
                BirthDate = user.BirthDate,
                TasksIDS = user.Tasks?.Select(t => t.ID).ToList() ?? new()
            };

            // Optionals
            if (expands is null) return dto;
            if (Helper.ExpandHelper.ShouldExpand(expands, "tasks"))
            {
                dto.Tasks = user.Tasks?
                .Select(t => new TaskSummaryDto
                {
                    ID = t.ID,
                    Name = t.Name,
                    Description = t.Description,
                })
                .ToList() ?? new List<TaskSummaryDto>();
            }
            return dto;
        }
    }
}
