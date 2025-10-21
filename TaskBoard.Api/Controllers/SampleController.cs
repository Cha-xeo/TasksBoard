using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public SampleController(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        // JWT-protected endpoint
        [HttpGet("jwt")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult JwtEndpoint() => Ok("JWT authorized");

        // API Key-protected endpoint
        [HttpGet("apikey")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public IActionResult ApiKeyEndpoint([FromHeader(Name = "X-API-KEY")] string key)
        {
            return Ok("API Key authorized");
        }

        // Either JWT or API Key
        [HttpGet("either")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",ApiKey")]
        public IActionResult EitherEndpoint() => Ok("JWT or API Key authorized");

        // Generate an ApiKey
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> createApiKey([FromBody]string name)
        {
            string newKey = await _apiKeyService.Create(name);
            return Ok(newKey);
        }
    }
}
