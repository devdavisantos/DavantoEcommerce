using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Application.Abstractions.Authentication
{
    public interface IJwtService
    {
        AccessTokenResult GenerateAccessToken(User user, DateTime? sessionExpiresAtUtc = null);
    }
}

