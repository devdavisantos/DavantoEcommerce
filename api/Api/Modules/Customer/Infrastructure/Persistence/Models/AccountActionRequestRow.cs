namespace Api.Modules.Customer.Infrastructure.Persistence.Models
{
    internal sealed class AccountActionRequestRow
    {
        public ulong Id { get; init; }

        public Guid ExternalId { get; init; }

        public ulong UserId { get; init; }

        public string Type { get; init; }
            = string.Empty;

        public string TokenHash { get; init; }
            = string.Empty;

        public string? Metadata { get; init; }

        public DateTime ExpiresAt { get; init; }

        public DateTime? CompletedAt { get; init; }

        public DateTime? RevokedAt { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}