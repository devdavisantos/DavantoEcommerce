using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Domain.Entities;
using Api.Modules.Catalog.Infrastructure.Mappings;
using Api.Shared.Database;
using Dapper;
using MySqlConnector;

namespace Api.Modules.Catalog.Infrastructure.Persistence
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly IDbSession _dbSession;

        public ProductRepository(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<IReadOnlyCollection<string>> GetSlugsByPrefixAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT slug
                FROM catalogs.products
                WHERE slug = @Slug
                   OR slug LIKE CONCAT(@Slug, '-%');
                """;

            var command = new CommandDefinition(
                sql,
                new { Slug = slug },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var slugs = await _dbSession.Connection.QueryAsync<string>(
                command);

            return slugs.AsList();
        }

        public async Task<bool> SkuExistsAsync(
            string sku,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM catalogs.products
                    WHERE sku = @Sku
                );
                """;

            var command = new CommandDefinition(
                sql,
                new { Sku = sku },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.ExecuteScalarAsync<bool>(
                command);
        }

        public async Task CreateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var productId = await InsertProductAsync(
                    product,
                    cancellationToken);

                await InsertImagesAsync(
                    productId,
                    product.Images,
                    cancellationToken);

                await _dbSession.CommitAsync(
                    cancellationToken);
            }
            catch (MySqlException exception)
                when (exception.ErrorCode == MySqlErrorCode.DuplicateKeyEntry &&
                      IsConstraint(exception, "uq_products_slug"))
            {
                await RollbackAsync();

                throw new ProductPersistenceException(
                    ProductPersistenceFailure.DuplicateSlug,
                    exception);
            }
            catch (MySqlException exception)
                when (exception.ErrorCode == MySqlErrorCode.DuplicateKeyEntry &&
                      IsConstraint(exception, "uq_products_sku"))
            {
                await RollbackAsync();

                throw new ProductPersistenceException(
                    ProductPersistenceFailure.DuplicateSku,
                    exception);
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        private async Task<long> InsertProductAsync(
            Product product,
            CancellationToken cancellationToken)
        {
            const string sql = """
                INSERT INTO catalogs.products (
                    external_id,
                    category_id,
                    brand_id,
                    title,
                    slug,
                    description,
                    sku,
                    price,
                    discount_price,
                    stock,
                    `condition`,
                    status,
                    created_at,
                    updated_at
                )
                VALUES (
                    @ExternalId,
                    @CategoryId,
                    @BrandId,
                    @Title,
                    @Slug,
                    @Description,
                    @Sku,
                    @Price,
                    @DiscountPrice,
                    @Stock,
                    @Condition,
                    @Status,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT LAST_INSERT_ID();
                """;

            var parameters = new
            {
                product.ExternalId,
                product.CategoryId,
                product.BrandId,
                product.Title,
                product.Slug,
                product.Description,
                product.Sku,
                product.Price,
                product.DiscountPrice,
                product.Stock,
                Condition = ProductPersistenceMapper.ToDatabase(
                    product.Condition),
                Status = ProductPersistenceMapper.ToDatabase(
                    product.Status),
                product.CreatedAt,
                product.UpdatedAt
            };

            var command = new CommandDefinition(
                sql,
                parameters,
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection.QuerySingleAsync<long>(
                command);
        }

        private async Task InsertImagesAsync(
            long productId,
            IReadOnlyCollection<ProductImage> images,
            CancellationToken cancellationToken)
        {
            if (images.Count == 0)
                return;

            const string sql = """
                INSERT INTO catalogs.product_images (
                    external_id,
                    product_id,
                    image_key,
                    is_primary,
                    sort_order,
                    created_at,
                    updated_at
                )
                VALUES (
                    @ExternalId,
                    @ProductId,
                    @ImageKey,
                    @IsPrimary,
                    @SortOrder,
                    @CreatedAt,
                    @UpdatedAt
                );
                """;

            var parameters = images.Select(image => new
            {
                image.ExternalId,
                ProductId = productId,
                image.ImageKey,
                image.IsPrimary,
                image.SortOrder,
                image.CreatedAt,
                image.UpdatedAt
            });

            var command = new CommandDefinition(
                sql,
                parameters,
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection.ExecuteAsync(
                command);
        }

        private async Task RollbackAsync()
        {
            try
            {
                await _dbSession.RollbackAsync(
                    CancellationToken.None);
            }
            catch
            {
                // Preserve the original persistence exception.
            }
        }

        private static bool IsConstraint(
            MySqlException exception,
            string constraintName)
        {
            return exception.Message.Contains(
                constraintName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}