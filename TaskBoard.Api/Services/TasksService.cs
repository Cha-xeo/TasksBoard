using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services
{
    public class TasksService : ITasksService
    {
        private readonly TasksContext _tasksContext;
        public TasksService(TasksContext tasksContext)
        {
            _tasksContext = tasksContext;
        }

        // Return a list of all tasks as TaskDtos or null if not found
        public async Task<IEnumerable<TaskDto>?> GetAllTasks()
        {
            List<Tasks>? tasks = await _tasksContext.Tasks.ToListAsync();
            return tasks.Count == 0 ? null : tasks.Select(t => MapToDto(t)).ToList();
        }

        // Return a task as TaskDto corresponding to the ID or null if not found
        public async Task<TaskDto?> GetById(int ID)
        {
            Tasks? task = await _tasksContext.Tasks.FindAsync(ID);
            return task is null ? null : MapToDto(task);
        }
        public async Task<TaskDto> Create(Tasks newTask)
        {
            try
            {
                _tasksContext.Tasks.Add(newTask);
                await _tasksContext.SaveChangesAsync();
                return MapToDto(newTask);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                throw;
            }
        }
        public async Task<TaskDto?> Update(int ID, Tasks UpdatedTask)
        {
            Tasks? toUpdate = await _tasksContext.Tasks.FindAsync(ID);
            if (toUpdate is null) return null;

            if (UpdatedTask.ID != toUpdate.ID)
                throw new ApiException("Missmatched IDs", 400);
            _tasksContext.Tasks.Entry(toUpdate).CurrentValues.SetValues(UpdatedTask);
            await _tasksContext.SaveChangesAsync();
            return toUpdate is null ? null: MapToDto(toUpdate);
        }
        public async Task<bool> Delete(int ID)
        {
           Tasks? toDelete = await _tasksContext.Tasks.FindAsync(ID);
            if (toDelete is null) return false;
            _tasksContext.Tasks.Remove(toDelete);
            await _tasksContext.SaveChangesAsync();
            return true;
        }
        public TaskDto MapToDto(Tasks task)
        {
            return new TaskDto
            {
                ID = task.ID,
                UserIDS = task.UserIDS,
                Name = task.Name,
                Description = task.Description,
            };
        }
    }
}
