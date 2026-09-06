using Api.Controllers.Extensions;
using Api.Controllers.Requests.Auth;
using Api.Controllers.Requests.Customer;
using Api.Controllers.Requests.Customers;
using Api.Modules.Customer.Application.Address.Create;
using Api.Modules.Customer.Application.Address.Delete;
using Api.Modules.Customer.Application.Address.GetAll;
using Api.Modules.Customer.Application.Address.SetDefault;
using Api.Modules.Customer.Application.Address.Update;
using Api.Modules.Customer.Application.Auth.ChangePassword;
using Api.Modules.Customer.Application.Auth.Delete;
using Api.Modules.Customer.Application.Auth.ForgotPassword;
using Api.Modules.Customer.Application.Auth.ResetPassword;
using Api.Modules.Customer.Application.Email.Change;
using Api.Modules.Customer.Application.Email.Confirm;
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
        private readonly CreateAddressHandler _createAddressHandler;
        private readonly GetAddressesHandler _getAddressesHandler;
        private readonly UpdateAddressHandler _updateAddressHandler;
        private readonly SetDefaultAddressHandler _setDefaultAddressHandler;
        private readonly DeleteAddressHandler _deleteAddressHandler;
        private readonly ChangeEmailHandler _changeEmailHandler;
        private readonly ConfirmEmailChangeHandler _confirmEmailChangeHandler;
        private readonly ForgotPasswordHandler _forgotPasswordHandler;
        private readonly ResetPasswordHandler _resetPasswordHandler;

        public CustomerController(
            GetProfileHandler getProfileHandler,
            DeleteHandler deleteHandler,
            ChangePasswordHandler changePasswordHandler,
            UpdateProfileHandler updateProfileHandler,
            CreateAddressHandler createAddressHandler,
            GetAddressesHandler getAddressesHandler,
            UpdateAddressHandler updateAddressHandler,
            SetDefaultAddressHandler setDefaultAddressHandler,
            DeleteAddressHandler deleteAddressHandler,
            ChangeEmailHandler changeEmailHandler,
            ConfirmEmailChangeHandler confirmEmailChangeHandler,
            ForgotPasswordHandler forgotPasswordHandler,
            ResetPasswordHandler resetPasswordHandler)
        {
            _getProfileHandler = getProfileHandler;
            _deleteHandle = deleteHandler;
            _changePasswordHandler = changePasswordHandler;
            _updateProfileHandler = updateProfileHandler;
            _createAddressHandler = createAddressHandler;
            _getAddressesHandler = getAddressesHandler;
            _updateAddressHandler = updateAddressHandler;
            _setDefaultAddressHandler = setDefaultAddressHandler;
            _deleteAddressHandler = deleteAddressHandler;
            _changeEmailHandler = changeEmailHandler;
            _confirmEmailChangeHandler = confirmEmailChangeHandler;
            _forgotPasswordHandler = forgotPasswordHandler;
            _resetPasswordHandler = resetPasswordHandler;
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

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail(
        [FromBody] ChangeEmailRequest request,
        CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null ||
                !Guid.TryParse(
                    externalIdClaim.Value,
                    out var userExternalId))
            {
                return Unauthorized();
            }

            var command =
                new ChangeEmailCommand(
                    userExternalId,
                    request.NewEmail);

            var result =
                await _changeEmailHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return Ok(new
            {
                token = result.Value
            });
        }

        [AllowAnonymous]
        [HttpPost("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange(
        [FromBody] ConfirmEmailChangeRequest request,
        CancellationToken cancellationToken)
        {
            var command =
                new ConfirmEmailChangeCommand(
                    request.Token);

            var result =
                await _confirmEmailChangeHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return NoContent();
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null ||
                !Guid.TryParse(
                    externalIdClaim.Value,
                    out var userExternalId))
            {
                return Unauthorized();
            }

            var query = new GetAddressesQuery(
                userExternalId);

            var result =
                await _getAddressesHandler.HandleAsync(
                    query,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> CreateAddress(
        [FromBody] CreateAddressRequest request,
        CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null ||
                !Guid.TryParse(
                    externalIdClaim.Value,
                    out var userExternalId))
            {
                return Unauthorized();
            }

            var command = new CreateAddressCommand(
                userExternalId,
                request.Name,
                request.RecipientName,
                request.PostalCode,
                request.Street,
                request.Number,
                request.Complement,
                request.District,
                request.City,
                request.State,
                request.Country,
                request.IsDefault);

            var result =
                await _createAddressHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return Created(
                $"/api/customers/addresses/{result.Value.ExternalId}",
                result.Value);
        }

        [HttpPatch("addresses/{addressExternalId:guid}")]
        public async Task<IActionResult> UpdateAddress(
            Guid addressExternalId,
            [FromBody] UpdateAddressRequest request,
            CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null ||
                !Guid.TryParse(
                    externalIdClaim.Value,
                    out var userExternalId))
            {
                return Unauthorized();
            }

            var command = new UpdateAddressCommand(
                userExternalId,
                addressExternalId,

                HasValue(request.Name),
                GetStringValue(request.Name),

                HasValue(request.RecipientName),
                GetStringValue(request.RecipientName),

                HasValue(request.PostalCode),
                GetStringValue(request.PostalCode),

                HasValue(request.Street),
                GetStringValue(request.Street),

                HasValue(request.Number),
                GetStringValue(request.Number),

                HasValue(request.Complement),
                GetStringValue(request.Complement),

                HasValue(request.District),
                GetStringValue(request.District),

                HasValue(request.City),
                GetStringValue(request.City),

                HasValue(request.State),
                GetStringValue(request.State),

                HasValue(request.Country),
                GetStringValue(request.Country));

            var result =
                await _updateAddressHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return NoContent();
        }

        [HttpPatch("addresses/{addressExternalId:guid}/default")]
        public async Task<IActionResult> SetDefaultAddress(
            Guid addressExternalId,
            CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null ||
                !Guid.TryParse(
                    externalIdClaim.Value,
                    out var userExternalId))
            {
                return Unauthorized();
            }

            var command =
                new SetDefaultAddressCommand(
                    userExternalId,
                    addressExternalId);

            var result =
                await _setDefaultAddressHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return NoContent();
        }

        [HttpDelete("addresses/{addressExternalId:guid}")]
        public async Task<IActionResult> DeleteAddress(
            Guid addressExternalId,
            CancellationToken cancellationToken)
        {
            var externalIdClaim = User.FindFirst(
                JwtRegisteredClaimNames.Sub);

            if (externalIdClaim is null ||
                !Guid.TryParse(
                    externalIdClaim.Value,
                    out var userExternalId))
            {
                return Unauthorized();
            }

            var command =
                new DeleteAddressCommand(
                    userExternalId,
                    addressExternalId);

            var result =
                await _deleteAddressHandler.HandleAsync(
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

        private static bool HasValue(JsonElement element)
        {
            return element.ValueKind !=
                JsonValueKind.Undefined;
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var command =
                new ForgotPasswordCommand(
                    request.Email);

            var result = await _forgotPasswordHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return Ok(new
            {
                token = result.Value
            });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var command =
                new ResetPasswordCommand(
                    request.Token,
                    request.NewPassword);

            var result =
                await _resetPasswordHandler.HandleAsync(
                    command,
                    cancellationToken);

            if (result.IsFailure)
            {
                return this.ToActionResult(
                    result.Error);
            }

            return NoContent();
        }

    }
}
