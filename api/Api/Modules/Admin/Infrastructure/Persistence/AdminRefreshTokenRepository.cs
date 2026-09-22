using Api.Modules.Admin.Domain.Entities;
using Api.Modules.Admin.Domain.Repositories;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Admin.Infrastructure.Persistence
{
    public sealed class AdminRefreshTokenRepository : IAdminRefreshTokenRepository
    {
        private readonly IDbSession _dbSession;

        public AdminRefreshTokenRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<AdminRefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    user_id AS UserId,
                    token_hash AS TokenHash,
                    expires_at AS ExpiresAt,
                    revoked_at AS RevokedAt,
                    replaced_at AS ReplacedAt,
                    created_at AS CreatedAt,
                    device AS Device,
                    ip_address AS IpAddress
                FROM admins.refresh_tokens
                WHERE token_hash = @TokenHash
                LIMIT 1;
                """;

            var command = new CommandDefinition(
                sql,
                new { TokenHash = tokenHash },
                _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection
                .QuerySingleOrDefaultAsync<AdminRefreshTokenRow>(command);

            return row?.ToDomain();
        }

        public async Task<long> CreateAsync(
            AdminRefreshToken refreshToken,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO admins.refresh_tokens (
                    user_id,
                    token_hash,
                    expires_at,
                    revoked_at,
                    replaced_at,
                    created_at,
                    device,
                    ip_address
                )
                VALUES (
                    @UserId,
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

            var command = new CommandDefinition(
                sql,
                new
                {
                    refreshToken.UserId,
                    refreshToken.TokenHash,
                    refreshToken.ExpiresAt,
                    refreshToken.RevokedAt,
                    refreshToken.ReplacedAt,
                    refreshToken.CreatedAt,
                    refreshToken.Device,
                    refreshToken.IpAddress
                },
                _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection
                .ExecuteScalarAsync<long>(command);
        }

        public async Task<bool> ReplaceAsync(
            AdminRefreshToken currentToken,
            AdminRefreshToken newToken,
            CancellationToken cancellationToken = default)
        {
            // Compare-and-swap: only the request holding the current hash can rotate.
            // Keep the session id, original deadline, creation date and login metadata.
            const string sql = """
                UPDATE admins.refresh_tokens
                SET token_hash = @NewTokenHash
                WHERE id = @Id
                  AND token_hash = @CurrentTokenHash
                  AND revoked_at IS NULL
                  AND replaced_at IS NULL
                  AND expires_at > UTC_TIMESTAMP();
                """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    currentToken.Id,
                    CurrentTokenHash = currentToken.TokenHash,
                    NewTokenHash = newToken.TokenHash
                },
                _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.ExecuteAsync(command) == 1;
        }
        public async Task RevokeAllByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE admins.refresh_tokens
                SET revoked_at = @RevokedAt
                WHERE user_id = @UserId
                  AND revoked_at IS NULL
                  AND replaced_at IS NULL;
                """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    RevokedAt = DateTime.UtcNow
                },
                _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection.ExecuteAsync(command);
        }

        public async Task RevokeAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                 UPDATE admins.refresh_tokens
                 SET revoked_at = @RevokedAt
                 WHERE id = @Id
                   AND revoked_at IS NULL
                   AND replaced_at IS NULL;
                 """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    Id = id,
                    RevokedAt = DateTime.UtcNow
                },
                _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var affectedRows = await _dbSession.Connection.ExecuteAsync(command);

            if (affectedRows != 1)
            {
                throw new InvalidOperationException(
                    "Admin refresh token could not be revoked.");
            }
        }

    }
}


