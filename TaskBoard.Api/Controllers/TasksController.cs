using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services.Interface;

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
        public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] string? expand = "", [FromQuery] bool summary = false)
        {
            var expands = expand?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            var tasks = await _taskService.GetAllTasks<object>(expands, summary);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? expand = "", [FromQuery] bool summary = false)
        {
            var expands = expand?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            var task = await _taskService.GetById(id, expands, summary);
            if (task is null) return BadRequest();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateDto newTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TaskDto createdTask = await _taskService.Create(newTask);

            return CreatedAtAction(nameof(GetById), new {id = createdTask.ID}, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto updatedTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TaskDto? task = await _taskService.Update(id, updatedTask);
            if (task is null) return BadRequest();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            bool state = await _taskService.Delete(id);
            if (state == false) BadRequest(state);
            return Ok(state);
        }
    }
}
