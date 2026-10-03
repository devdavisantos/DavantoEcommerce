using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Application.Abstractions.Storage;
using Api.Modules.Catalog.Application.Products.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Catalog.Application.Products.GetById
{
    public sealed class GetProductByIdHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IObjectStorage _objectStorage;

        public GetProductByIdHandler(
            IProductRepository productRepository,
            IObjectStorage objectStorage)
        {
            _productRepository = productRepository;
            _objectStorage = objectStorage;
        }

        public async Task<Result<ProductDetails>> HandleAsync(
            GetProductByIdQuery query,
            CancellationToken cancellationToken = default)
        {
            if (query.ExternalId == Guid.Empty)
            {
                return Result.Failure<ProductDetails>(
                    ProductApplicationErrors.ProductNotFound);
            }

            var product = await _productRepository
                .GetByExternalIdAsync(
                    query.ExternalId,
                    cancellationToken);

            if (product is null)
            {
                return Result.Failure<ProductDetails>(
                    ProductApplicationErrors.ProductNotFound);
            }

            var images = product.Images
                .OrderBy(image => image.SortOrder)
                .Select(image => new ProductImageDetails(
                    image.ExternalId,
                    _objectStorage.GetPublicUrl(
                        image.ImageKey),
                    image.IsPrimary,
                    image.SortOrder))
                .ToArray();

            return Result.Success(
                new ProductDetails(
                    product.ExternalId,
                    product.CategoryExternalId,
                    product.Category,
                    product.BrandExternalId,
                    product.Brand,
                    product.Title,
                    product.Slug,
                    product.Description,
                    product.Sku,
                    product.Price,
                    product.DiscountPrice,
                    product.Stock,
                    product.Condition,
                    product.Status,
                    images,
                    product.CreatedAt,
                    product.UpdatedAt));
        }
    }
}