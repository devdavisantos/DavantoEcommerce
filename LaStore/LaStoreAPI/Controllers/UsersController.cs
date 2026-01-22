using LaStoreAPI.Data;
using LaStoreAPI.DTOs;
using LaStoreAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LaStoreAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPut("me")]
        public async Task<IActionResult>Update(UpdateUserRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _context.Users.FindAsync(Guid.Parse(userId!));
            user!.Name = request.Name;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
