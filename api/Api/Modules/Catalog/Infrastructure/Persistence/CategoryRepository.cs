using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Domain.Entities;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Catalog.Infrastructure.Persistence
{
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly IDbSession _dbSession;

        public CategoryRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<Category?> GetByExternalIdAsync(
            Guid externalId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id,
                    external_id AS ExternalId,
                    name,
                    slug,
                    active,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM catalogs.categories
                WHERE external_id = @ExternalId
                LIMIT 1;
                """;

            var command = new CommandDefinition(
                sql,
                new { ExternalId = externalId },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row = await _dbSession.Connection
                .QuerySingleOrDefaultAsync<CategoryRow>(command);

            if (row is null)
                return null;

            return Category.Restore(
                row.Id,
                row.ExternalId,
                row.Name,
                row.Slug,
                row.Active,
                row.CreatedAt,
                row.UpdatedAt);
        }

        private sealed record CategoryRow(
            uint Id,
            Guid ExternalId,
            string Name,
            string Slug,
            bool Active,
            DateTime CreatedAt,
            DateTime UpdatedAt);
    }
}