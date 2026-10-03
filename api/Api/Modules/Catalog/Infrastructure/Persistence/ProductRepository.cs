using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Domain.Entities;
using Api.Modules.Catalog.Infrastructure.Mappings;
using Api.Modules.Catalog.Application.Products.GetAll;
using Api.Modules.Catalog.Domain.Enums;
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

        public async Task<PagedResult<ProductListData>> GetAllAsync(
            ProductListFilter filter,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    p.external_id AS ExternalId,
                    p.title AS Title,
                    p.sku AS Sku,
                    p.price AS Price,
                    p.discount_price AS DiscountPrice,
                    p.stock AS Stock,
                    p.`condition` AS `Condition`,
                    p.status AS Status,
                    c.name AS Category,
                    b.name AS Brand,
                    pi.image_key AS PrimaryImageKey,
                    p.created_at AS CreatedAt
                FROM catalogs.products p
                LEFT JOIN catalogs.categories c
                    ON c.id = p.category_id
                LEFT JOIN catalogs.brands b
                    ON b.id = p.brand_id
                LEFT JOIN catalogs.product_images pi
                    ON pi.product_id = p.id
                   AND pi.is_primary = 1
                WHERE
                    (
                        @Search IS NULL
                        OR p.title LIKE CONCAT('%', @Search, '%')
                        OR p.sku LIKE CONCAT('%', @Search, '%')
                    )
                    AND (
                        @Status IS NULL
                        OR p.status = @Status
                    )
                ORDER BY p.created_at DESC, p.id DESC
                LIMIT @PageSize
                OFFSET @Offset;
                """;

                    const string countSql = """
                SELECT COUNT(*)
                FROM catalogs.products p
                WHERE
                    (
                        @Search IS NULL
                        OR p.title LIKE CONCAT('%', @Search, '%')
                        OR p.sku LIKE CONCAT('%', @Search, '%')
                    )
                    AND (
                        @Status IS NULL
                        OR p.status = @Status
                    );
                """;

            var search = string.IsNullOrWhiteSpace(filter.Search)
                ? null
                : filter.Search.Trim();

            var parameters = new
            {
                Search = search,
                Status = filter.Status.HasValue
                    ? ProductPersistenceMapper.ToDatabase(filter.Status.Value)
                    : null,
                filter.PageSize,
                Offset = (filter.Page - 1) * filter.PageSize
            };

            var countCommand = new CommandDefinition(
                countSql,
                parameters,
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var totalItems = await _dbSession.Connection
                .ExecuteScalarAsync<long>(countCommand);

            if (totalItems == 0)
            {
                return new PagedResult<ProductListData>(
                    [],
                    filter.Page,
                    filter.PageSize,
                    0);
            }

            var command = new CommandDefinition(
                sql,
                parameters,
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var rows = await _dbSession.Connection
                .QueryAsync<ProductListItemRow>(command);

            var items = rows
                .Select(row => new ProductListData(
                    row.ExternalId,
                    row.Title,
                    row.Sku,
                    row.Price,
                    row.DiscountPrice,
                    row.Stock,
                    ProductPersistenceMapper.ToProductCondition(row.Condition),
                    ProductPersistenceMapper.ToProductStatus(row.Status),
                    row.Category,
                    row.Brand,
                    row.PrimaryImageKey,
                    row.CreatedAt))
                .ToArray();

            return new PagedResult<ProductListData>(
                items,
                filter.Page,
                filter.PageSize,
                totalItems);
        }

        public async Task<ProductDetailsData?> GetByExternalIdAsync(
            Guid externalId,
            CancellationToken cancellationToken = default)
        {
            const string productSql = """
                SELECT
                    p.id AS Id,
                    p.external_id AS ExternalId,
                    c.external_id AS CategoryExternalId,
                    c.name AS Category,
                    b.external_id AS BrandExternalId,
                    b.name AS Brand,
                    p.title AS Title,
                    p.slug AS Slug,
                    p.description AS Description,
                    p.sku AS Sku,
                    p.price AS Price,
                    p.discount_price AS DiscountPrice,
                    p.stock AS Stock,
                    p.`condition` AS `Condition`,
                    p.status AS Status,
                    p.created_at AS CreatedAt,
                    p.updated_at AS UpdatedAt
                FROM catalogs.products p
                LEFT JOIN catalogs.categories c
                    ON c.id = p.category_id
                LEFT JOIN catalogs.brands b
                    ON b.id = p.brand_id
                WHERE p.external_id = @ExternalId
                LIMIT 1;
                """;

            var productCommand = new CommandDefinition(
                productSql,
                new
                {
                    ExternalId = externalId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var productRow = await _dbSession.Connection
                .QuerySingleOrDefaultAsync<ProductDetailsRow>(
                    productCommand);

            if (productRow is null)
                return null;

            const string imagesSql = """
                SELECT
                    external_id AS ExternalId,
                    image_key AS ImageKey,
                    is_primary AS IsPrimary,
                    sort_order AS SortOrder
                FROM catalogs.product_images
                WHERE product_id = @ProductId
                ORDER BY sort_order ASC, id ASC;
                """;

            var imagesCommand = new CommandDefinition(
                imagesSql,
                new
                {
                    ProductId = productRow.Id
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var imageRows = await _dbSession.Connection
                .QueryAsync<ProductImageDetailsRow>(
                    imagesCommand);

            var images = imageRows
                .Select(image => new ProductImageData(
                    image.ExternalId,
                    image.ImageKey,
                    image.IsPrimary,
                    image.SortOrder))
                .ToArray();

            return new ProductDetailsData(
                productRow.ExternalId,
                productRow.CategoryExternalId,
                productRow.Category,
                productRow.BrandExternalId,
                productRow.Brand,
                productRow.Title,
                productRow.Slug,
                productRow.Description,
                productRow.Sku,
                productRow.Price,
                productRow.DiscountPrice,
                productRow.Stock,
                ProductPersistenceMapper.ToProductCondition(productRow.Condition),
                ProductPersistenceMapper.ToProductStatus(productRow.Status),
                images,
                productRow.CreatedAt,
                productRow.UpdatedAt);
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

        private sealed record ProductListItemRow(
            Guid ExternalId,
            string Title,
            string? Sku,
            decimal Price,
            decimal? DiscountPrice,
            uint Stock,
            string Condition,
            string Status,
            string? Category,
            string? Brand,
            string? PrimaryImageKey,
            DateTime CreatedAt
        );

        private sealed record ProductDetailsRow(
            ulong Id,
            Guid ExternalId,
            Guid? CategoryExternalId,
            string? Category,
            Guid? BrandExternalId,
            string? Brand,
            string Title,
            string Slug,
            string Description,
            string? Sku,
            decimal Price,
            decimal? DiscountPrice,
            uint Stock,
            string Condition,
            string Status,
            DateTime CreatedAt,
            DateTime UpdatedAt);

        private sealed record ProductImageDetailsRow(
            Guid ExternalId,
            string ImageKey,
            bool IsPrimary,
            uint SortOrder);

    }
}