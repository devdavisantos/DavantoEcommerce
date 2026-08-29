using Api.Controllers.Requests.Auth;
using Api.Modules.Customer.Application.Auth.Register;
using Microsoft.AspNetCore.Mvc;
using Api.Modules.Customer.Application.Auth.Login;
using Microsoft.AspNetCore.Authorization;
using Api.Modules.Customer.Application.Auth.Refresh;
using Api.Modules.Customer.Application.Auth.Logout;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly RegisterHandler _registerHandler;
        private readonly LoginHandler _loginHandler;
        private readonly RefreshHandler _refreshHandler;
        private readonly LogoutHandler _logoutHandler;

        public AuthController(
            RegisterHandler registerHandler,
            LoginHandler loginHandler,
            RefreshHandler refreshHandler,
            LogoutHandler logoutHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
            _refreshHandler = refreshHandler;
            _logoutHandler = logoutHandler;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(
                request.Name,
                request.Email,
                request.Password);

            var result = await _registerHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error.Code ==
                    "Customer.Register.EmailAlreadyExists")
                {
                    return Conflict(new
                    {
                        result.Error.Code,
                        result.Error.Message
                    });
                }

                return BadRequest(new
                {
                    result.Error.Code,
                    result.Error.Message
                });
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _loginHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    result.Error.Code,
                    result.Error.Message
                });
            }

            return Ok(result.Value);

        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RefreshCommand(request.RefreshToken);

            var result = await _refreshHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    result.Error.Code,
                    result.Error.Message
                });
                    
            }

            return Ok(result.Value);

        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutRequest request,
            CancellationToken cancellationToken)
        {
            var command = new LogoutCommand(
                request.RefreshToken);

            var result = await _logoutHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    result.Error.Code,
                    result.Error.Message
                });
            }

            return NoContent();
        }

    }
}