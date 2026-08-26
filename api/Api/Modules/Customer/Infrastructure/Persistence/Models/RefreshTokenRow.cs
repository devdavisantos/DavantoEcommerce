namespace Api.Modules.Customer.Infrastructure.Persistence.Models
{
    internal sealed class RefreshTokenRow
    {
        public ulong Id { get; init; }
        public ulong LoginId { get; init; }
        public string TokenHash { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public DateTime? RevokedAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? Device { get; init; }
        public string? IpAddress { get; init; }
    }
}
