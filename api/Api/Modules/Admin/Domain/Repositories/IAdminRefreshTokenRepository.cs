using Api.Modules.Admin.Domain.Entities;

namespace Api.Modules.Admin.Domain.Repositories
{
    public interface IAdminRefreshTokenRepository
    {
        Task<AdminRefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task<long> CreateAsync(
            AdminRefreshToken refreshToken,
            CancellationToken cancellationToken = default);

        Task<bool> ReplaceAsync(
            AdminRefreshToken currentToken,
            AdminRefreshToken newToken,
            CancellationToken cancellationToken = default);

        Task RevokeAllByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task RevokeAsync(
            long id,
            CancellationToken cancellationToken = default);
    }
}
