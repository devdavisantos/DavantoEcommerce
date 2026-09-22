namespace Api.Modules.Admin.Domain.Entities
{
    public sealed class AdminRefreshToken
    {
        public long Id { get; private set; }
        public int UserId { get; private set; }
        public string TokenHash { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public DateTime? ReplacedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Device { get; private set; }
        public string? IpAddress { get; private set; }

        private AdminRefreshToken(
            long id,
            int userId,
            string tokenHash,
            DateTime expiresAt,
            DateTime? revokedAt,
            DateTime? replacedAt,
            DateTime createdAt,
            string? device,
            string? ipAddress)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc);
            RevokedAt = revokedAt;
            ReplacedAt = replacedAt;
            CreatedAt = createdAt;
            Device = device;
            IpAddress = ipAddress;
        }

        public static AdminRefreshToken Create(
            int userId,
            string tokenHash,
            DateTime expiresAt,
            string? device,
            string? ipAddress)
        {
            return new AdminRefreshToken(
                id: 0,
                userId: userId,
                tokenHash: tokenHash,
                expiresAt: expiresAt,
                revokedAt: null,
                replacedAt: null,
                createdAt: DateTime.UtcNow,
                device: device,
                ipAddress: ipAddress);
        }

        public static AdminRefreshToken Restore(
            long id,
            int userId,
            string tokenHash,
            DateTime expiresAt,
            DateTime? revokedAt,
            DateTime? replacedAt,
            DateTime createdAt,
            string? device,
            string? ipAddress)
        {
            return new AdminRefreshToken(
                id,
                userId,
                tokenHash,
                expiresAt,
                revokedAt,
                replacedAt,
                createdAt,
                device,
                ipAddress);
        }

        public bool IsExpired()
        {
            return ExpiresAt <= DateTime.UtcNow;
        }

        public bool IsRevoked()
        {
            return RevokedAt.HasValue;
        }

        public bool IsReplaced()
        {
            return ReplacedAt.HasValue;
        }

        public void Revoke()
        {
            if (RevokedAt.HasValue)
                return;

            RevokedAt = DateTime.UtcNow;
        }

        public void Replace()
        {
            if (ReplacedAt.HasValue)
                return;

            ReplacedAt = DateTime.UtcNow;
        }
    }
}
