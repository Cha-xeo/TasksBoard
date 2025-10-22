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
    // TODO fix task requiring full fledged user on update
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

            IQueryable<Tasks> query = _tasksContext.Tasks.AsQueryable();

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


            List<Tasks> tasks = await query.ToListAsync();

            //List<Tasks>? tasks = await _tasksContext.Tasks.ToListAsync();
            return tasks.Count == 0 ? null : tasks.Select(t => TaskMapper.ToDto(t, expands)).ToList();
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
            }


            Tasks? task = await query.FirstOrDefaultAsync(t => t.ID == ID);
            return task is null ? null : TaskMapper.ToDto(task, expands);
        }
        
        public async Task<TaskDto> Create(TaskCreateDto requestTask, List<string>? expands = null)
        {
            try
            {
                Tasks newTask = new Tasks
                {
                    Name = requestTask.Name,
                    Description = requestTask.Description,
                    CreatedAt = DateTime.UtcNow,
                    User = new List<Users>()
                };

                await ValidateUsersAsync(newTask, requestTask.UserIDS);

                _tasksContext.Tasks.Add(newTask);
                await _tasksContext.SaveChangesAsync();
                return TaskMapper.ToDto(newTask, expands);
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
        public async Task<TaskDto?> Update(int ID, TaskUpdateDto taskUpdate, List<string>? expands = null)
        {
            Tasks? existingTask = await _tasksContext.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.ID == ID);

            if (existingTask is null) return null;
            if (taskUpdate.ID != existingTask.ID)
                throw new ApiException("Missmatched IDs", 400);

            // Handle navigation values and validate usersId
            await ValidateUsersAsync(existingTask, taskUpdate.UserIDS);

            // handle scalar values
            TaskMapper.FromTaskUpdateDto(existingTask, taskUpdate);


            await _tasksContext.SaveChangesAsync();
            return TaskMapper.ToDto(existingTask, expands);
        }
        public async Task<bool> Delete(int ID)
        {
           Tasks? toDelete = await _tasksContext.Tasks.FindAsync(ID);
            if (toDelete is null) return false;
            _tasksContext.Tasks.Remove(toDelete);
            await _tasksContext.SaveChangesAsync();
            return true;
        }

        public async Task ValidateUsersAsync(Tasks newTask, List<int>? requestedIds)
        {
            List<Users>? existingRequestedUsers = null;

            if (requestedIds is not null)
            {
                existingRequestedUsers = await _tasksContext.Users
                    .Where(u => requestedIds.Contains(u.ID))
                    .ToListAsync();

                if (existingRequestedUsers.Count != requestedIds.Count)
                {
                    throw new ApiException("Some assigned users do not exist.", 400);
                }
            }

            Helper.ColletionHelpers.SyncCollection(newTask.User, existingRequestedUsers, u => u.ID);
        }
    }
}
