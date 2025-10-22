using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Handler;
using TaskBoard.Api.Models;
using TaskBoard.Api.Models.Api;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel?>> Login(LoginRequestModel request)
        {
            LoginResponseModel? resp = await _authService.Authenticate(request);
            if (resp is null) return Unauthorized();
            return Ok(resp);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto?>> Register(RegisterRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest("UserName is required");
            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password is required");
            UserDto? resp = await _authService.Register(request);
            if (resp is null) return BadRequest();
            return CreatedAtRoute("GetUserByID", new { id = resp.ID}, resp);
        }
    }
}
