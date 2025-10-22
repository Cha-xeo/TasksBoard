using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers([FromQuery] string? expand = "", [FromQuery] bool summary = false)
        {
            var expands = expand?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            var user = await _userService.GetAllUsers<object>(expands, summary);
            return Ok(user);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserByID(int id, [FromQuery] string? expand = "", [FromQuery] bool summary = false)
        {
            var expands = expand?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            var user = await _userService.GetById(id, expands, summary);
            if (user is null) return BadRequest();
            return Ok(user);
        }

        // Registration is now done through the Auth controller
        //[HttpPost]
        //public async Task<IActionResult> Create(Users user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var created = await _userService.Create(user);

        //    return CreatedAtAction(nameof(GetUserByID), new {id = created.ID}, created);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserByID(int id, UserUpdateDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UserDto? updatedUser = await _userService.Update(id, user);
            if (updatedUser is null) return BadRequest();
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserByID(int id) 
        {
            //bool state = await _userService.Delete(id);
            bool state = await _userService.SoftDelete(id);
            if (state == false) return BadRequest(state);
            return Ok(state);
        }

        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteUserByID(int id) 
        {
            //bool state = await _userService.Delete(id);
            bool state = await _userService.SoftDelete(id);
            if (state == false) return BadRequest(state);
            return Ok(state);
        }

        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreUserByID(int id) 
        {
            UserDto? updatedUser = await _userService.RestoreSoftDeleted(id);
            if (updatedUser is null) return BadRequest();
            return Ok(updatedUser);
        }
    }
}
