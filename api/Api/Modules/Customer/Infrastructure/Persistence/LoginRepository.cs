using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Persistence.Models;
using Api.Modules.Customer.Domain.Enums;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Customer.Infrastructure.Persistence
{
    public sealed class LoginRepository : ILoginRepository
    {
        private readonly IDbSession _dbSession;

        public LoginRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<Login?> GetActiveByUserIdAsync(ulong userId, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    user_id AS UserId,
                    password_hash AS PasswordHash,
                    status AS Status,
                    last_login_at AS LastLoginAt,
                    created_at AS CreatedAt
                FROM customer.logins
                WHERE user_id = @UserId
                AND status = 'ACTIVE'
                ORDER BY created_at DESC
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    UserId = userId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<LoginRow>(command);

            return row is null
            ? null
            : Map(row);
        }

        public async Task<Login?> GetByIdAsync(
        ulong id,
        CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                user_id AS UserId,
                password_hash AS PasswordHash,
                status AS Status,
                last_login_at AS LastLoginAt,
                created_at AS CreatedAt
            FROM customer.logins
            WHERE id = @Id
            LIMIT 1;
            """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<LoginRow>(command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<ulong> CreateAsync(
            Login login,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO customer.logins
            (
                user_id,
                password_hash,
                status,
                last_login_at,
                created_at
            )
            VALUES
            (
                @UserId,
                @PasswordHash,
                @Status,
                @LastLoginAt,
                @CreatedAt
            );

            SELECT LAST_INSERT_ID();
            """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    login.UserId,
                    login.PasswordHash,
                    Status = MapStatus(login.Status),
                    login.LastLoginAt,
                    login.CreatedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.ExecuteScalarAsync<ulong>(command);
        }

        public async Task UpdateLastLoginAsync(
            Login login,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE customer.logins
            SET last_login_at = @LastLoginAt
            WHERE id = @Id
              AND status = 'ACTIVE';
            """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    login.Id,
                    login.LastLoginAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        public async Task DeactivateAsync(
            Login login,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE customer.logins
            SET status = @Status
            WHERE id = @Id;
            """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    login.Id,
                    Status = MapStatus(login.Status)
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection.ExecuteAsync(command);
        }

        private static Login Map(LoginRow row)
        {
            return Login.Restore(
                row.Id,
                row.UserId,
                row.PasswordHash,
                MapStatus(row.Status),
                row.LastLoginAt,
                row.CreatedAt);
        }

        private static LoginStatus MapStatus(string status)
        {
            return status switch
            {
                "ACTIVE" => LoginStatus.Active,
                "INACTIVE" => LoginStatus.Inactive,

                _ => throw new InvalidOperationException(
                    $"Unknown login status: {status}")
            };
        }

        private static string MapStatus(LoginStatus status)
        {
            return status switch
            {
                LoginStatus.Active => "ACTIVE",
                LoginStatus.Inactive => "INACTIVE",

                _ => throw new ArgumentOutOfRangeException(nameof(status))
            };
        }

    }
}
