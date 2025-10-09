using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TaskBoard.Api.Services
{
    public class TasksService : ITasksService
    {
        private readonly TasksContext _tasksContext;

        // Map expansion keys (from query or service) to EF Core navigation expressions
        private static readonly Dictionary<string, Expression<Func<Tasks, object>>> ExpansionMap
            = new()
                {
                    { "users", t => t.User }
                };

        public TasksService(TasksContext tasksContext)
        {
            _tasksContext = tasksContext;
        }

        // Return a list of all tasks as TaskDtos or null if not found
        public async Task<IEnumerable<TaskDto>?> GetAllTasks(List<string>? expands = null)
        {
            List<Tasks>? tasks = await _tasksContext.Tasks.ToListAsync();
            return tasks.Count == 0 ? null : tasks.Select(t => MapToDto(t, expands)).ToList();
        }

        // Return a task as TaskDto corresponding to the ID or null if not found
        public async Task<TaskDto?> GetById(int ID, List<string>? expands = null)
        {   
            IQueryable<Tasks> query = _tasksContext.Tasks.AsQueryable();

            if (expands is not null)
            {
                foreach (string? key in expands)
                {
                    if (ExpansionMap.TryGetValue(key, out var expression))
                    {
                        query = query.Include(expression);
                    }
                }
                //if (expands.Contains("users", StringComparer.OrdinalIgnoreCase)) 
                //{
                //    query = query.Include(t => t.User);
                //}
            }


            Tasks? task = await query.FirstOrDefaultAsync(t => t.ID == ID);
            return task is null ? null : MapToDto(task, expands);
        }
        
        public async Task<TaskDto> Create(Tasks newTask, List<string>? expands = null)
        {
            try
            {
                await ValidateUsersAsync(newTask);

                _tasksContext.Tasks.Add(newTask);
                await _tasksContext.SaveChangesAsync();
                return MapToDto(newTask, expands);
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
        public async Task<TaskDto?> Update(int ID, Tasks UpdatedTask, List<string>? expands = null)
        {
            // TODO Change to query.FirstOrDefaultAsync(t => t.ID == ID) when changing deletion method to soft delete
            Tasks? toUpdate = await _tasksContext.Tasks.FindAsync(ID);
            if (toUpdate is null) return null;

            if (UpdatedTask.ID != toUpdate.ID)
                throw new ApiException("Missmatched IDs", 400);

            await ValidateUsersAsync(UpdatedTask);

            _tasksContext.Tasks.Entry(toUpdate).CurrentValues.SetValues(UpdatedTask);
            await _tasksContext.SaveChangesAsync();
            return toUpdate is null ? null: MapToDto(toUpdate, expands);
        }
        public async Task<bool> Delete(int ID)
        {
           Tasks? toDelete = await _tasksContext.Tasks.FindAsync(ID);
            if (toDelete is null) return false;
            _tasksContext.Tasks.Remove(toDelete);
            await _tasksContext.SaveChangesAsync();
            return true;
        }

        public async Task ValidateUsersAsync(Models.Tasks newTask)
        {
            if (newTask.UserIDS.IsNullOrEmpty()) return;

            var existingUsers = await _tasksContext.Users
                    .Where(u => newTask.UserIDS.Contains(u.ID))
                    .ToListAsync();

            if (existingUsers.Count != newTask.UserIDS?.Count)
            {
                throw new ArgumentException("Some assigned users do not exist.");
            }

            newTask.User = existingUsers;
        }

        public TaskDto MapToDto(Tasks task, List<string>? expands = null)
        {
            // Default
            TaskDto dto = new TaskDto
            {
                ID = task.ID,
                UserIDS = task.UserIDS,
                Name = task.Name,
                Description = task.Description,
            };

            // Optionals
            
            if (expands is null) return dto;

            if (ExpandHelper.ShouldExpand(expands, "users"))
            {
                dto.Users = task.User?
                .Select(u => new UserSummaryDto
                {
                    ID = u.ID,
                    UserName = u.UserName,
                    BirthDate = u.BirthDate
                })
                .ToList() ?? new List<UserSummaryDto>();    
            }

            return dto;
        }
    }
}
