using Microsoft.AspNetCore.Mvc;
using LaStoreAPI.Services;
using LaStoreAPI.DTOs;
using LaStoreAPI.Security;
using System.Reflection.Metadata.Ecma335;

namespace LaStoreAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register(RegisterRequest request)
        {
            await _authService.Register(request);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginRequest request)
        {
            var user = await _authService.Login(request);
            var token = JwtTokenGenerator.Generate(user, HttpContext.RequestServices
                .GetRequiredService<IConfiguration>());

            return Ok(new { token });
        }
    }
}
