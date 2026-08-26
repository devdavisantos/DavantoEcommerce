using Api.Controllers.Requests.Auth;
using Api.Modules.Customer.Application.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request,
            [FromServices] RegisterHandler handler,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(
                request.Name,
                request.Email,
                request.Password);

            var result = await handler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error.Code ==
                    "Customers.Register.EmailAlreadyExists")
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
    }
}