using System.IdentityModel.Tokens.Jwt;
using Api.Modules.Customer.Application.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public sealed class CustomerController : ControllerBase
    {
        private readonly GetProfileHandler _getProfileHandler;

        public CustomerController(
            GetProfileHandler getProfileHandler)
        {
            _getProfileHandler = getProfileHandler;
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
                return NotFound(new
                {
                    result.Error.Code,
                    result.Error.Message
                });
            }

            return Ok(result.Value);
        }

    }
}
