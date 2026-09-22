using Api.Modules.Admin.Domain.Entities;
using Api.Modules.Admin.Domain.Enums;

namespace Api.Modules.Admin.Infrastructure.Persistence
{
    internal sealed class AdminUserRow
    {
        public int Id { get; init; }
        public Guid ExternalId { get; init; }
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string PasswordHash { get; init; } = null!;
        public string Status { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        public AdminUser ToDomain()
        {
            return AdminUser.Restore(
                Id,
                ExternalId,
                Name,
                Email,
                PasswordHash,
                Enum.Parse<AdminUserStatus>(Status, ignoreCase: true),
                CreatedAt,
                UpdatedAt);
        }
    }
}