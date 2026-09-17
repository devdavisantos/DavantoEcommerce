using Api.Controllers.Requests.Auth;
using Api.Modules.Customer.Application.Auth.Register;
using Microsoft.AspNetCore.Mvc;
using Api.Modules.Customer.Application.Auth.Login;
using Microsoft.AspNetCore.Authorization;
using Api.Modules.Customer.Application.Auth.Refresh;
using Api.Modules.Customer.Application.Auth.Logout;
using Api.Controllers.Extensions;
using System.IdentityModel.Tokens.Jwt;

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
                return this.ToActionResult(result.Error);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            // Always overwrite metadata from the request body with server-observed values.
            var peer = LoginDevice.NormalizeIp(HttpContext.Connection.RemoteIpAddress?.ToString());
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var trustedPeers = configuration.GetSection("LoginProxy:KnownProxies").Get<string[]>() ?? [];
            var trustedProxy = peer != null && trustedPeers.Any(ip => LoginDevice.NormalizeIp(ip) == peer);
            var fromFrontend = Request.Headers["X-Lastore-Proxy"] == "1";
            var ip = fromFrontend
                ? trustedProxy ? LoginDevice.NormalizeIp(Request.Headers["X-Lastore-Client-IP"].ToString()) : null
                : peer;
            command = command with {
                Device = LoginDevice.Describe(Request.Headers.UserAgent.ToString()),
                IpAddress = ip
            };
            var result = await _loginHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(result.Error);
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
                return this.ToActionResult(result.Error);
                    
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
                return this.ToActionResult(result.Error);
            }

            return NoContent();
        }
            
    }
}
