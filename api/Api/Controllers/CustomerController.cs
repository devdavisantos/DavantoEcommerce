using Api.Controllers.Extensions;
using Api.Controllers.Requests.Customer;
using Api.Modules.Customer.Application.Auth.ChangePassword;
using Api.Modules.Customer.Application.Auth.Delete;
using Api.Modules.Customer.Application.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

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
        private readonly UpdateProfileHandler _updateProfileHandler;

        public CustomerController(
            GetProfileHandler getProfileHandler,
            DeleteHandler deleteHandler,
            ChangePasswordHandler changePasswordHandler,
            UpdateProfileHandler updateProfileHandler)
        {
            _getProfileHandler = getProfileHandler;
            _deleteHandle = deleteHandler;
            _changePasswordHandler = changePasswordHandler;
            _updateProfileHandler = updateProfileHandler;
        }

        #region Profile
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

        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request,
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

            var hasName = request.Name.ValueKind != JsonValueKind.Undefined;
            var hasDocument = request.Document.ValueKind != JsonValueKind.Undefined;
            var hasPhone = request.Phone.ValueKind != JsonValueKind.Undefined;

            var name = GetStringValue(request.Name);
            var document = GetStringValue(request.Document);
            var phone = GetStringValue(request.Phone);

            var command = new UpdateProfileCommand(
                externalId,
                hasName,
                name,
                hasDocument,
                document,
                hasPhone,
                phone);

            var result = await _updateProfileHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return NoContent();
        }

        #endregion

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

        private static string? GetStringValue(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Undefined ||
                    element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (element.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return element.GetString();
        }

    }
}
