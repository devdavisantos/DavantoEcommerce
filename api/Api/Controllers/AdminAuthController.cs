using Api.Controllers.Requests.Auth;
using Api.Controllers.Responses;
using Api.Modules.Admin.Application.Auth.Login;
using Api.Modules.Admin.Application.Auth.Logout;
using Api.Modules.Admin.Application.Auth.Refresh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/admin/auth")]
    public sealed class AdminAuthController : ControllerBase
    {
        private readonly AdminLoginHandler _loginHandler;
        private readonly AdminRefreshHandler _refreshHandler;
        private readonly AdminLogoutHandler _logoutHandler;

        public AdminAuthController(
            AdminLoginHandler loginHandler,
            AdminRefreshHandler refreshHandler,
            AdminLogoutHandler logoutHandler)
        {
            _loginHandler = loginHandler;
            _refreshHandler = refreshHandler;
            _logoutHandler = logoutHandler;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] AdminLoginRequest request,
            CancellationToken cancellationToken)
        {
            var peer = LoginDevice.NormalizeIp(HttpContext.Connection.RemoteIpAddress?.ToString());
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var trustedPeers = configuration.GetSection("LoginProxy:KnownProxies").Get<string[]>() ?? [];
            var trustedProxy = peer != null && trustedPeers.Any(ip => LoginDevice.NormalizeIp(ip) == peer);
            var ip = Request.Headers["X-Lastore-Proxy"] == "1"
                ? trustedProxy ? LoginDevice.NormalizeIp(Request.Headers["X-Lastore-Client-IP"].ToString()) : null
                : peer;
            var command = new AdminLoginCommand(
                request.Email,
                request.Password,
                LoginDevice.Describe(Request.Headers.UserAgent.ToString()),
                ip);

            var result = await _loginHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                var response = new
                {
                    code = result.Error.Code,
                    message = result.Error.Message
                };

                if (result.Error.Code == AdminLoginErrors.ValidationCode)
                    return BadRequest(response);

                return Unauthorized(response);
            }

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] AdminRefreshRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AdminRefreshCommand(request.RefreshToken, null, null);

            var result = await _refreshHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return Unauthorized(new
                {
                    code = result.Error.Code,
                    message = result.Error.Message
                });
            }

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] AdminLogoutRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AdminLogoutCommand(
                request.RefreshToken);

            var result = await _logoutHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return Unauthorized(new ApiErrorResponse(
                    result.Error.Code,
                    result.Error.Message));
            }

            return NoContent();
        }

    }

    public sealed record AdminLoginRequest(
        string Email,
        string Password);

    public sealed record AdminRefreshRequest(
        string RefreshToken);
    public sealed record AdminLogoutRequest(
    string RefreshToken);
}
