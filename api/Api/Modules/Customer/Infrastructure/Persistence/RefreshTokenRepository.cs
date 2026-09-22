using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Infrastructure.Persistence.Models;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Customer.Infrastructure.Persistence
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbSession _dbSession;

        public RefreshTokenRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                login_id AS LoginId,
                token_hash AS TokenHash,
                expires_at AS ExpiresAt,
                revoked_at AS RevokedAt,
                replaced_at AS ReplacedAt,
                created_at AS CreatedAt,
                device AS Device,
                ip_address AS IpAddress
            FROM customer.refresh_tokens
            WHERE token_hash = @TokenHash
            LIMIT 1;
            """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    TokenHash = tokenHash
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection
                .QuerySingleOrDefaultAsync<RefreshTokenRow>(command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<ulong> CreateAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO customer.refresh_tokens
            (
                login_id,
                token_hash,
                expires_at,
                revoked_at,
                replaced_at,
                created_at,
                device,
                ip_address
            )
            VALUES
            (
                @LoginId,
                @TokenHash,
                @ExpiresAt,
                @RevokedAt,
                @ReplacedAt,
                @CreatedAt,
                @Device,
                @IpAddress
            );

            SELECT LAST_INSERT_ID();
            """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    refreshToken.LoginId,
                    refreshToken.TokenHash,
                    refreshToken.ExpiresAt,
                    refreshToken.RevokedAt,
                    refreshToken.ReplacedAt,
                    refreshToken.CreatedAt,
                    refreshToken.Device,
                    refreshToken.IpAddress
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection
                .ExecuteScalarAsync<ulong>(command);
        }

        public async Task RevokeAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE customer.refresh_tokens
            SET revoked_at = @RevokedAt
            WHERE id = @Id;
            """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    refreshToken.Id,
                    refreshToken.RevokedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        public async Task RevokeAllByLoginIdAsync(
            ulong loginId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE customer.refresh_tokens
            SET revoked_at = UTC_TIMESTAMP()
            WHERE login_id = @LoginId
              AND revoked_at IS NULL
              AND replaced_at IS NULL;
            """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    LoginId = loginId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    login_id AS LoginId,
                    token_hash AS TokenHash,
                    expires_at AS ExpiresAt,
                    revoked_at AS RevokedAt,
                    replaced_at AS ReplacedAt,
                    created_at AS CreatedAt,
                    device AS Device,
                    ip_address AS IpAddress
                FROM customer.refresh_tokens
                WHERE token_hash = @RefreshToken
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    RefreshToken = refreshToken
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken
                );
            
            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<RefreshTokenRow>(command);

            if (row is null)
                return null!;

            return RefreshToken.Restore(
                row.Id,
                row.LoginId,
                row.TokenHash,
                row.ExpiresAt,
                row.RevokedAt,
                row.ReplacedAt,
                row.CreatedAt,
                row.Device,
                row.IpAddress);
        }

        public async Task RevokeAsync(
            ulong id,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.refresh_tokens
                SET revoked_at = UTC_TIMESTAMP()
                WHERE id = @Id
                    AND revoked_at IS NULL
                    AND replaced_at IS NULL;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var affectedRows = await _dbSession.Connection.ExecuteAsync(command);

            if (affectedRows != 1)
            {
                throw new InvalidOperationException(
                    "Refresh token could not be revoked.");
            }
        }


        public async Task<bool> ReplaceAsync(
            ulong id,
            DateTime replacedAt,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.refresh_tokens
                SET replaced_at = @ReplacedAt
                WHERE id = @Id
                    AND replaced_at IS NULL
                    AND revoked_at IS NULL
                    AND expires_at > UTC_TIMESTAMP();
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Id = id,
                    ReplacedAt = replacedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var affectedRows =
                await _dbSession.Connection.ExecuteAsync(command);

            return affectedRows == 1;
        }

        private static RefreshToken Map(
            RefreshTokenRow row)
        {
            return RefreshToken.Restore(
                row.Id,
                row.LoginId,
                row.TokenHash,
                row.ExpiresAt,
                row.RevokedAt,
                row.ReplacedAt,
                row.CreatedAt,
                row.Device,
                row.IpAddress);
        }
    }
}
