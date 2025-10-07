
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using TaskBoard.Api.Dtos;

namespace TaskBoard.Api.Services
{
    public interface ITasksService
    {
        Task<IEnumerable<TaskDto>?> GetAllTasks();
        Task<TaskDto?> GetById(int ID);
        Task<TaskDto> Create(Models.Tasks newTask);
        Task<TaskDto?> Update(int ID, Models.Tasks UpdatedTask);
        Task<bool> Delete(int ID);
        TaskDto MapToDto(Models.Tasks DeletedTask);
    }
}
