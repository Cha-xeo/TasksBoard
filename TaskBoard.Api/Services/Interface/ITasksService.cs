
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services.Interface
{
    public interface ITasksService
    {
        Task<IEnumerable<TaskDto>?> GetAllTasks(List<string>? expands = null);
        Task<TaskDto?> GetById(int ID, List<string>? expands = null);
        Task<TaskDto> Create(TaskCreateDto newTask, List<string>? expands = null);
        Task<TaskDto?> Update(int ID, TaskUpdateDto UpdatedTask, List<string>? expands = null);
        Task<bool> Delete(int ID);

        Task ValidateUsersAsync(Tasks newTask, List<int>? requestedIds);
    }
}
