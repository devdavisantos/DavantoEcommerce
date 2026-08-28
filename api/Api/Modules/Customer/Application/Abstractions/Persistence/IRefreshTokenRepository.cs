using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Application.Abstractions.Persistence
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task ReplaceAsync(ulong id, DateTime replacedAt, CancellationToken cancellationToken);
        Task RevokeByLoginIdAsync(ulong id, CancellationToken cancellationToken);
    }
}
