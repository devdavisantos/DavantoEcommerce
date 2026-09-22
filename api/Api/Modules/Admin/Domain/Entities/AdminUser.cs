using Api.Modules.Admin.Domain.Enums;

namespace Api.Modules.Admin.Domain.Entities
{
    public sealed class AdminUser
    {
        public int Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public AdminUserStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private AdminUser(
            int id,
            Guid externalId,
            string name,
            string email,
            string passwordHash,
            AdminUserStatus status,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public static AdminUser Create(
            string name,
            string email,
            string passwordHash)
        {

            return new AdminUser(
                id: 0,
                externalId: Guid.NewGuid(),
                name: name.Trim(),
                email: email.Trim().ToLowerInvariant(),
                passwordHash: passwordHash,
                status: AdminUserStatus.Active,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow);
        }

        public static AdminUser Restore(
            int id,
            Guid externalId,
            string name,
            string email,
            string passwordHash,
            AdminUserStatus status,
            DateTime createdAt,
            DateTime updatedAt)
        {
            return new AdminUser(
                id,
                externalId,
                name,
                email,
                passwordHash,
                status,
                createdAt,
                updatedAt);
        }

        public void Activate()
        {
            if (Status == AdminUserStatus.Active)
                return;

            Status = AdminUserStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Inactivate()
        {
            if (Status == AdminUserStatus.Inactive)
                return;

            Status = AdminUserStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}