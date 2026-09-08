namespace Api.Modules.Customer.Infrastructure.Persistence.Models
{
    internal sealed class AddressRow
    {
        public ulong Id { get; init; }
        public Guid ExternalId { get; init; }
        public ulong UserId { get; init; }
        public string? Name { get; init; }
        public string RecipientName { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string? Number { get; init; }
        public string? Complement { get; init; }
        public string? District { get; init; }
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}