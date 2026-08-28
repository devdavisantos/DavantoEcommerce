using Api.Controllers.Requests.Auth;
using Api.Modules.Customer.Application.Auth.Register;
using Microsoft.AspNetCore.Mvc;
using Api.Modules.Customer.Application.Auth.Login;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly RegisterHandler _registerHandler;
        private readonly LoginHandler _loginHandler;

        public AuthController(RegisterHandler registerHandler, LoginHandler loginHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
        }


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

    }
}