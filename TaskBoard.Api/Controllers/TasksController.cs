using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetById(int ID)
        {
            TaskDto? task = await _taskService.GetById(ID);
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
        public async Task<IActionResult> UpdateTask(int ID, Tasks updatedTask)
        {
            TaskDto? task = await _taskService.Update(ID, updatedTask);
            if (task is null) return NotFound();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int ID)
        {
            bool state = await _taskService.Delete(ID);
            if (state == false) NotFound(state);
            return Ok(state);
        }
    }
}
