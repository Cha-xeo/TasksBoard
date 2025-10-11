using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services.Interface;

// TODO Add expand to other route or adding specialized route for details
namespace TaskBoard.Api.Controllers
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _taskService;

        public TasksController(ITasksService tasksService)
        {
            _taskService = tasksService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<TaskDto>? tasks = await _taskService.GetAllTasks();
            if (tasks is null) return NotFound();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? expand = "")
        {
            var expands = expand?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            TaskDto? task = await _taskService.GetById(id, expands);
            if (task is null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(Tasks newTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TaskDto createdTask = await _taskService.Create(newTask);

            return CreatedAtAction(nameof(GetById), new {id = createdTask.ID}, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, Tasks updatedTask)
        {
            TaskDto? task = await _taskService.Update(id, updatedTask);
            if (task is null) return NotFound();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            bool state = await _taskService.Delete(id);
            if (state == false) NotFound(state);
            return Ok(state);
        }
    }
}
