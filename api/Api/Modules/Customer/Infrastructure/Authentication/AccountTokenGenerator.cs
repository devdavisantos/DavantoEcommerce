using Api.Modules.Customer.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace Api.Modules.Customer.Infrastructure.Authentication
{
    public sealed class AccountTokenGenerator : IAccountTokenGenerator
    {
        public string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return WebEncoders.Base64UrlEncode (bytes);
        }

        public string Hash(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}