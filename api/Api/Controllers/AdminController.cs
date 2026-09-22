using Api.Modules.Admin.Application.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public sealed class AdminController : ControllerBase
    {
        private readonly GetAdminProfileHandler _getProfileHandler;

        public AdminController(
            GetAdminProfileHandler getProfileHandler)
        {
            _getProfileHandler = getProfileHandler;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var sub = User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(sub, out var externalId))
                return Unauthorized();

            var profile = await _getProfileHandler.HandleAsync(
                externalId);

            if (profile is null)
                return Unauthorized();

            return Ok(profile);
        }
    }
}