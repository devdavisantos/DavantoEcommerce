using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Persistence.Models;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Customer.Infrastructure.Persistence
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbSession _dbSession;
        public UserRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<User?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    stripe_customer_id AS StripeCustomerId,
                    name AS Name,
                    email AS Email,
                    document AS Document,
                    phone AS Phone,
                    status AS Status,
                    email_confirmed_at AS EmailConfirmedAt,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt,
                    deleted_at AS DeletedAt
                FROM customer.users
                WHERE Id = @Id
                AND deleted_at IS NULL
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<UserRow>(command);

            return row is null
                ? null 
                : Map(row);
        }
        
        public async Task<User?> GetByExternalIdAsync(Guid externalId,  CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    stripe_customer_id AS StripeCustomerId,
                    name AS Name,
                    email AS Email,
                    document AS Document,
                    phone AS Phone,
                    status AS Status,
                    email_confirmed_at AS EmailConfirmedAt,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt,
                    deleted_at AS DeletedAt
                FROM customer.users
                WHERE external_id = @ExternalId
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    ExternalId = externalId.ToString()
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<UserRow> (command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    stripe_customer_id AS StripeCustomerId,
                    name AS Name,
                    email AS Email,
                    document AS Document,
                    phone AS Phone,
                    status AS Status,
                    email_confirmed_at AS EmailConfirmedAt,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt,
                    deleted_at AS DeletedAt
                FROM customer.users
                WHERE email = @Email
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Email = email.Trim().ToLowerInvariant()
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<UserRow>(command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM customer.users
                    WHERE email = @Email
                );
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Email = email.Trim().ToLowerInvariant()
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.ExecuteScalarAsync<bool>(command);
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            ulong excludeUserId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT EXISTS(
                    SELECT 1
                    FROM customer.users
                    WHERE email = @Email
                      AND id <> @ExcludeUserId
                );
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Email = email,
                    ExcludeUserId = excludeUserId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection
                .ExecuteScalarAsync<bool>(
                    command);
        }

        public async Task<ulong> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            const string insertSql = """
                INSERT INTO customer.users
                (
                    external_id,
                    stripe_customer_id,
                    name,
                    email,
                    document,
                    phone,
                    status,
                    email_confirmed_at,
                    created_at,
                    updated_at,
                    deleted_at
                )
                VALUES
                (
                    @ExternalId,
                    @StripeCustomerId,
                    @Name,
                    @Email,
                    @Document,
                    @Phone,
                    @Status,
                    @EmailConfirmedAt,
                    @CreatedAt,
                    @UpdatedAt,
                    @DeletedAt
                )
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var insertCommand = new CommandDefinition(
                insertSql,
                new
                {
                    ExternalId = user.ExternalId.ToString(),
                    user.StripeCustomerId,
                    user.Name,
                    user.Email,
                    user.Document,
                    user.Phone,
                    Status = (byte)user.Status,
                    user.EmailConfirmedAt,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.DeletedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection.ExecuteAsync(insertCommand);

            const string idSql =
                "SELECT LAST_INSERT_ID();";

            var idCommand = new CommandDefinition(
                idSql,
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.ExecuteScalarAsync<ulong>(idCommand);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.users
                SET
                    stripe_customer_id = @StripeCustomerId,
                    name = @Name,
                    email = @Email,
                    document = @Document,
                    phone = @Phone,
                    status = @Status,
                    email_confirmed_at = @EmailConfirmedAt,
                    updated_at = @UpdatedAt,
                    deleted_at = @DeletedAt
                WHERE id = @Id;
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    user.Id,
                    user.StripeCustomerId,
                    user.Name,
                    user.Email,
                    user.Document,
                    user.Phone,
                    Status = (byte)user.Status,
                    user.EmailConfirmedAt,
                    user.UpdatedAt,
                    user.DeletedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection.ExecuteScalarAsync(command);
        }

        public async Task<bool> DocumentExistsAsync(
            string document,
            ulong excludeUserId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT EXISTS(
                    SELECT 1
                    FROM customer.users
                    WHERE document = @Document
                        AND id <> @ExcludeUserId
                );
                """;

            await _dbSession.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Document = document,
                    ExcludeUserId = excludeUserId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.ExecuteScalarAsync<bool>(command);
        }

        private static User Map(UserRow row)
        {
            return User.Restore(
                row.Id,
                Guid.Parse(row.ExternalId),
                row.StripeCustomerId,
                row.Name,
                row.Email,
                row.Document,
                row.Phone,
                (UserStatus)row.Status,
                row.EmailConfirmedAt,
                row.CreatedAt,
                row.UpdatedAt,
                row.DeleteAt);
        }
    }
}