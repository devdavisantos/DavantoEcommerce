using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Persistence.Models;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Customer.Infrastructure.Persistence
{
    public sealed class AccountActionRequestRepository
        : IAccountActionRequestRepository
    {
        private readonly IDbSession _dbSession;

        public AccountActionRequestRepository(
            IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<AccountActionRequest?>
            GetByTokenHashAsync(
                string tokenHash,
                string type,
                CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    user_id AS UserId,
                    type AS Type,
                    token_hash AS TokenHash,
                    metadata AS Metadata,
                    expires_at AS ExpiresAt,
                    completed_at AS CompletedAt,
                    revoked_at AS RevokedAt,
                    created_at AS CreatedAt
                FROM customer.account_action_requests
                WHERE token_hash = @TokenHash
                  AND type = @Type
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    TokenHash = tokenHash,
                    Type = type
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row =
                await _dbSession.Connection
                    .QuerySingleOrDefaultAsync<
                        AccountActionRequestRow>(
                        command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<ulong> CreateAsync(
            AccountActionRequest request,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO customer.account_action_requests
                (
                    external_id,
                    user_id,
                    type,
                    token_hash,
                    metadata,
                    expires_at,
                    completed_at,
                    revoked_at,
                    created_at
                )
                VALUES
                (
                    @ExternalId,
                    @UserId,
                    @Type,
                    @TokenHash,
                    @Metadata,
                    @ExpiresAt,
                    @CompletedAt,
                    @RevokedAt,
                    @CreatedAt
                );

                SELECT LAST_INSERT_ID();
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    request.ExternalId,
                    request.UserId,
                    request.Type,
                    request.TokenHash,
                    request.Metadata,
                    request.ExpiresAt,
                    request.CompletedAt,
                    request.RevokedAt,
                    request.CreatedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection
                .ExecuteScalarAsync<ulong>(
                    command);
        }

        public async Task UpdateAsync(
            AccountActionRequest request,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.account_action_requests
                SET
                    completed_at = @CompletedAt,
                    revoked_at = @RevokedAt
                WHERE id = @Id;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    request.Id,
                    request.CompletedAt,
                    request.RevokedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        public async Task RevokePendingByUserIdAndTypeAsync(
            ulong userId,
            string type,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.account_action_requests
                SET revoked_at = UTC_TIMESTAMP()
                WHERE user_id = @UserId
                  AND type = @Type
                  AND completed_at IS NULL
                  AND revoked_at IS NULL;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    Type = type
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        private static AccountActionRequest Map(
            AccountActionRequestRow row)
        {
            return AccountActionRequest.Restore(
                row.Id,
                row.ExternalId,
                row.UserId,
                row.Type,
                row.TokenHash,
                row.Metadata,
                row.ExpiresAt,
                row.CompletedAt,
                row.RevokedAt,
                row.CreatedAt);
        }
    }
}