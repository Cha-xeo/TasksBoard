
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using TaskBoard.Api.Dtos;

namespace TaskBoard.Api.Services
{
    public interface ITasksService
    {
        Task<IEnumerable<TaskDto>?> GetAllTasks(List<string>? expands = null);
        Task<TaskDto?> GetById(int ID, List<string>? expands = null);
        Task<TaskDto> Create(Models.Tasks newTask, List<string>? expands = null);
        Task<TaskDto?> Update(int ID, Models.Tasks UpdatedTask, List<string>? expands = null);
        Task<bool> Delete(int ID);

        Task ValidateUsersAsync(Models.Tasks newTask);
        TaskDto MapToDto(Models.Tasks task, List<string>? expands = null);
    }
}
