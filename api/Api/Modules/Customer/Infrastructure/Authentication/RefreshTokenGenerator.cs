using System.Security.Cryptography;
using System.Text;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Configuration;
using Microsoft.Extensions.Options;

namespace Api.Modules.Customer.Infrastructure.Authentication
{
    public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenGenerator(
            IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public RefreshTokenResult Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            var token = Convert.ToBase64String(bytes);

            var expiresAtUtc = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays);

            return new RefreshTokenResult(token, expiresAtUtc);
        }

        public string Hash(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
