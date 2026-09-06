using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Entities
{
    public sealed class AccountActionRequest
    {
        public ulong Id { get; private set; }

        public Guid ExternalId { get; private set; }

        public ulong UserId { get; private set; }

        public string Type { get; private set; }
            = string.Empty;

        public string TokenHash { get; private set; }
            = string.Empty;

        public string? Metadata { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public DateTime? RevokedAt { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private AccountActionRequest()
        {
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpiresAt;
        }

        public bool IsCompleted()
        {
            return CompletedAt.HasValue;
        }

        public bool IsRevoked()
        {
            return RevokedAt.HasValue;
        }

        public Result Complete()
        {
            if (IsCompleted())
            {
                return Result.Failure(
                    AccountActionRequestErrors.AlreadyCompleted);
            }

            if (IsRevoked())
            {
                return Result.Failure(
                    AccountActionRequestErrors.Revoked);
            }

            if (IsExpired())
            {
                return Result.Failure(
                    AccountActionRequestErrors.Expired);
            }

            CompletedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Revoke()
        {
            if (IsCompleted() || IsRevoked())
            {
                return Result.Success();
            }

            RevokedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public static Result<AccountActionRequest> Create(
            ulong userId,
            string type,
            string tokenHash,
            string? metadata,
            DateTime expiresAt)
        {
            if (userId == 0)
            {
                return Result.Failure<AccountActionRequest>(
                    AccountActionRequestErrors.InvalidUser);
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                return Result.Failure<AccountActionRequest>(
                    AccountActionRequestErrors.InvalidType);
            }

            if (string.IsNullOrWhiteSpace(tokenHash))
            {
                return Result.Failure<AccountActionRequest>(
                    AccountActionRequestErrors.InvalidToken);
            }

            if (expiresAt <= DateTime.UtcNow)
            {
                return Result.Failure<AccountActionRequest>(
                    AccountActionRequestErrors.InvalidExpiration);
            }

            var request =
                new AccountActionRequest
                {
                    ExternalId = Guid.NewGuid(),
                    UserId = userId,
                    Type = type.Trim(),
                    TokenHash = tokenHash,
                    Metadata = metadata,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                };

            return Result.Success(request);
        }

        public static AccountActionRequest Restore(
            ulong id,
            Guid externalId,
            ulong userId,
            string type,
            string tokenHash,
            string? metadata,
            DateTime expiresAt,
            DateTime? completedAt,
            DateTime? revokedAt,
            DateTime createdAt)
        {
            return new AccountActionRequest
            {
                Id = id,
                ExternalId = externalId,
                UserId = userId,
                Type = type,
                TokenHash = tokenHash,
                Metadata = metadata,
                ExpiresAt = expiresAt,
                CompletedAt = completedAt,
                RevokedAt = revokedAt,
                CreatedAt = createdAt
            };
        }
    }
}