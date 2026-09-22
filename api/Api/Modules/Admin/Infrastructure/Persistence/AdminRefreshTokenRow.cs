using Api.Modules.Admin.Domain.Entities;

namespace Api.Modules.Admin.Infrastructure.Persistence
{
    internal sealed class AdminRefreshTokenRow
    {
        public long Id { get; init; }
        public int UserId { get; init; }
        public string TokenHash { get; init; } = null!;
        public DateTime ExpiresAt { get; init; }
        public DateTime? RevokedAt { get; init; }
        public DateTime? ReplacedAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? Device { get; init; }
        public string? IpAddress { get; init; }

        public AdminRefreshToken ToDomain()
        {
            return AdminRefreshToken.Restore(
                Id,
                UserId,
                TokenHash,
                ExpiresAt,
                RevokedAt,
                ReplacedAt,
                CreatedAt,
                Device,
                IpAddress);
        }
    }
}