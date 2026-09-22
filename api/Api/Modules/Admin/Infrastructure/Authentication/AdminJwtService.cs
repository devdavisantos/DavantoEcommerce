using Api.Modules.Admin.Application.Abstractions.Authentication;
using Api.Modules.Admin.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Modules.Admin.Infrastructure.Authentication
{
    public sealed class AdminJwtService : IAdminJwtService
    {
        private readonly AdminJwtSettings _settings;

        public AdminJwtService(IOptions<AdminJwtSettings> options)
        {
            _settings = options.Value;
        }

        public AdminAccessTokenResult Generate(
            AdminUser user,
            DateTime sessionExpiresAtUtc)
        {
            var now = DateTime.UtcNow;
            var accessTokenExpiresAt = now.AddMinutes(_settings.AccessTokenExpirationMinutes);
            var expiresAt = accessTokenExpiresAt < sessionExpiresAtUtc
                ? accessTokenExpiresAt
                : sessionExpiresAtUtc;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ExternalId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("user_type", "ADMIN")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return new AdminAccessTokenResult(
                tokenValue,
                expiresAt);
        }
    }
}