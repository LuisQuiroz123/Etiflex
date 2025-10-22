using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Authentication;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AccessManagerService _accessManager;

        public AuthController(AccessManagerService accessManager)
        {
            _accessManager = accessManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApplicationCredentials credentials)
        {
            try
            {
                var token = await _accessManager.GetAccessTokenAsync(credentials);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = "INVALID_CREDENTIALS", message = ex.Message });
            }
        }
    }
}