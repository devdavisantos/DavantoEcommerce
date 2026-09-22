using Api.Modules.Admin.Domain.Entities;
using Api.Modules.Admin.Domain.Repositories;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Admin.Infrastructure.Persistence
{
    public sealed class AdminUserRepository : IAdminUserRepository
    {
        private readonly IDbSession _dbSession;

        public AdminUserRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<AdminUser?> GetByIdAsync(int id)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    name AS Name,
                    email AS Email,
                    password_hash AS PasswordHash,
                    status AS Status,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM admins.users
                WHERE id = @Id
                LIMIT 1;
                """;

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<AdminUserRow>(
                sql,
                new { Id = id },
                _dbSession.Transaction);

            return row?.ToDomain();
        }

        public async Task<AdminUser?> GetByExternalIdAsync(Guid externalId)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    name AS Name,
                    email AS Email,
                    password_hash AS PasswordHash,
                    status AS Status,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM admins.users
                WHERE external_id = @ExternalId
                LIMIT 1;
                """;

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<AdminUserRow>(
                sql,
                new { ExternalId = externalId.ToString() },
                _dbSession.Transaction);

            return row?.ToDomain();
        }

        public async Task<AdminUser?> GetByEmailAsync(string email)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    name AS Name,
                    email AS Email,
                    password_hash AS PasswordHash,
                    status AS Status,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM admins.users
                WHERE email = @Email
                LIMIT 1;
                """;

            var row = await _dbSession.Connection.QuerySingleOrDefaultAsync<AdminUserRow>(
                sql,
                new { Email = email.Trim().ToLowerInvariant() },
                _dbSession.Transaction);

            return row?.ToDomain();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM admins.users
                    WHERE email = @Email
                );
                """;

            return await _dbSession.Connection.ExecuteScalarAsync<bool>(
                sql,
                new { Email = email.Trim().ToLowerInvariant() },
                _dbSession.Transaction);
        }

        public async Task<int> CreateAsync(AdminUser user)
        {
            const string sql = """
                INSERT INTO admins.users (
                    external_id,
                    name,
                    email,
                    password_hash,
                    status,
                    created_at,
                    updated_at
                )
                VALUES (
                    @ExternalId,
                    @Name,
                    @Email,
                    @PasswordHash,
                    @Status,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT LAST_INSERT_ID();
                """;

            return await _dbSession.Connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    ExternalId = user.ExternalId.ToString(),
                    user.Name,
                    user.Email,
                    user.PasswordHash,
                    Status = user.Status.ToString().ToUpperInvariant(),
                    user.CreatedAt,
                    user.UpdatedAt
                },
                _dbSession.Transaction);
        }

    }
}