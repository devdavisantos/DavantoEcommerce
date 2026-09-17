using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Configuration;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Modules.Customer.Infrastructure.Authentication
{
    public sealed class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public AccessTokenResult GenerateAccessToken(User user, DateTime? sessionExpiresAtUtc = null)
        {
            var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.ExternalId.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _settings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(
                _settings.AccessTokenExpirationMinutes);

            if (sessionExpiresAtUtc.HasValue && expiration > sessionExpiresAtUtc.Value)
                expiration = sessionExpiresAtUtc.Value;

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return new AccessTokenResult(tokenValue, expiration);
        }
    }
}
