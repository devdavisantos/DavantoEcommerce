namespace Api.Modules.Customer.Infrastructure.Persistence.Models
{
    internal sealed class UserRow
    {
        public ulong Id { get; init; }
        public string ExternalId { get; init; } = string.Empty;
        public string? StripeCustomerId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Document { get; init; }
        public string? Phone { get; init; }
        public byte Status { get; init; }
        public DateTime? EmailConfirmedAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public DateTime? DeleteAt { get; init; }
    }
}
