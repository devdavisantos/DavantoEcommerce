using Api.Modules.Customer.Domain.Enums;

namespace Api.Modules.Customer.Infrastructure.Persistence.Models
{
    internal sealed class LoginRow
    {
        public ulong Id { get; init; }
        public ulong UserId { get; init; }
        public string PasswordHash { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime? LastLoginAt { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
