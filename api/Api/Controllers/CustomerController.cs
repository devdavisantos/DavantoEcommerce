using Api.Controllers.Extensions;
using Api.Controllers.Requests.Auth;
using Api.Modules.Customer.Application.Auth.ChangePassword;
using Api.Modules.Customer.Application.Auth.Delete;
using Api.Modules.Customer.Application.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/customer")]
    [Authorize]
    public sealed class CustomerController : ControllerBase
    {
        private readonly GetProfileHandler _getProfileHandler;
        private readonly DeleteHandler _deleteHandle;
        private readonly ChangePasswordHandler _changePasswordHandler;

        public CustomerController(
            GetProfileHandler getProfileHandler,
            DeleteHandler deleteHandler,
            ChangePasswordHandler changePasswordHandler)
        {
            _getProfileHandler = getProfileHandler;
            _deleteHandle = deleteHandler;
            _changePasswordHandler = changePasswordHandler;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(
            CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null || !Guid.TryParse(
                externalIdClaim.Value,
                out var externalId))
            {
                return Unauthorized();
            }

            var query = new GetProfileQuery(externalId);

            var result = await _getProfileHandler.HandleAsync(
                query,
                cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpDelete("account")]
        public async Task<IActionResult> DeleteAccount(
            CancellationToken cancellation)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null || !Guid.TryParse(
                externalIdClaim.Value,
                out var externalId))
            {
                return Unauthorized();
            }

            var command = new DeleteCommand(externalId);

            var result = await _deleteHandle.HandleAsync(
                command,
                cancellation);

            if (result.IsFailure)
            {
                return this.ToActionResult(result.Error);
            }

            return NoContent();

        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null || !Guid.TryParse(
                externalIdClaim.Value,
                out var externalId))
            {
                return Unauthorized();
            }

            var command = new ChangePasswordCommand(
                externalId,
                request.CurrentPassword,
                request.NewPassword);

            var result = await _changePasswordHandler.HandleAsync(
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
