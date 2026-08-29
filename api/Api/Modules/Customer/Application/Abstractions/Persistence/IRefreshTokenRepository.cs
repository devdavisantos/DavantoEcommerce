using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Application.Abstractions.Persistence
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<ulong> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<bool> ReplaceAsync(ulong id, DateTime replacedAt, CancellationToken cancellationToken);
        Task RevokeByLoginIdAsync(ulong id, CancellationToken cancellationToken);
        Task RevokeAsync(ulong id, CancellationToken cancellationToken);
    }
}
