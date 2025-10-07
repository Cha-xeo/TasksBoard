using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            var user = await _userService.getAllUsersAsync();
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var user = await _userService.getUserByIDAsync(id);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var created = await _userService.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUserByID), new {id = created.ID}, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserByID(int id, Users user)
        {
            UserDto? updatedUser = await _userService.UpdateUserAsync(id, user);
            if (updatedUser is null) return NotFound();
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserByID(int id) 
        {
            bool state = await _userService.DeleteUserAsync(id);
            if (state == false) return NotFound(state);
            return Ok(state);
        }
    }
}
