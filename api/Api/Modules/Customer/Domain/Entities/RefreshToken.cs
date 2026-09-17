using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Entities
{
    public sealed class RefreshToken
    {
        public ulong Id { get; private set; }
        public ulong LoginId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public DateTime? ReplacedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Device { get; private set; }
        public string? IpAddress { get; private set; }

        private RefreshToken()
        {
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpiresAt;
        }
        public bool IsRevoked()
        {
            return RevokedAt. HasValue;
        }
        public bool IsReplaced()
        {
            return ReplacedAt.HasValue;
        }

        public static Result<RefreshToken> Create(ulong loginId, string tokenHash, DateTime expiresAt, string? device, string? ipAddress)
        {
            if (loginId == 0)
            {
                return Result.Failure<RefreshToken>(
                    RefreshTokenErrors.InvalidLoginId);
            }

            if (string.IsNullOrWhiteSpace(tokenHash))
            {
                return Result.Failure<RefreshToken>(
                    RefreshTokenErrors.InvalidTokenHash);
            }

            if (expiresAt <= DateTime.UtcNow)
            {
                return Result.Failure<RefreshToken>(
                    RefreshTokenErrors.InvalidExpiration);
            }

            var refreshToken = new RefreshToken
            {
                LoginId = loginId,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc),
                RevokedAt = null,
                ReplacedAt = null,
                CreatedAt = DateTime.UtcNow,
                Device = string.IsNullOrWhiteSpace(device) ? null : device.Trim(),
                IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress.Trim()
            };

            return Result.Success(refreshToken);
        }

        public static RefreshToken Restore(
            ulong id,
            ulong loginId,
            string tokenHash,
            DateTime expiresAt,
            DateTime? revokedAt,
            DateTime? replacedAt,
            DateTime createdAt,
            string? device,
            string? ipAddress)
        {
            return new RefreshToken
            {
                Id = id,
                LoginId = loginId,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc),
                RevokedAt = revokedAt,
                ReplacedAt= replacedAt,
                CreatedAt = createdAt,
                Device = device,
                IpAddress = ipAddress
            };
        }

        public Result Revoke()
        {
            if (IsRevoked())
                return Result.Success();

            RevokedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Replace()
        {
            if (IsReplaced())
                return Result.Success();

            ReplacedAt = DateTime.UtcNow;

            return Result.Success();
        }

    }
}
