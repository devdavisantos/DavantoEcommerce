using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<ulong> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAllByLoginIdAsync(ulong loginId, CancellationToken cancellationToken = default);
    }
}
