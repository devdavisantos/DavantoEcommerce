using Api.Modules.Admin.Domain.Entities;

namespace Api.Modules.Admin.Application.Abstractions.Authentication
{
    public interface IAdminJwtService
    {
        AdminAccessTokenResult Generate(
            AdminUser user,
            DateTime sessionExpiresAtUtc);
    }

    public sealed record AdminAccessTokenResult(
        string Token,
        DateTime ExpiresAtUtc);
}