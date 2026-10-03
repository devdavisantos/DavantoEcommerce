using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Application.Abstractions.Storage;
using Api.Modules.Catalog.Application.Products.Errors;
using Api.Shared.Domain;
using FluentValidation;

namespace Api.Modules.Catalog.Application.Products.GetAll
{
    public sealed class GetProductsHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<GetProductsQuery> _validator;
        private readonly IObjectStorage _objectStorage;

        public GetProductsHandler(
            IProductRepository productRepository,
            IValidator<GetProductsQuery> validator,
            IObjectStorage objectStorage)
        {
            _productRepository = productRepository;
            _validator = validator;
            _objectStorage = objectStorage;
        }

        public async Task<Result<PagedResult<ProductListItem>>> HandleAsync(
            GetProductsQuery query,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(
                query,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<PagedResult<ProductListItem>>(
                    ProductApplicationErrors.ValidationFailed);
            }

            var filter = new ProductListFilter(
                query.Page,
                query.PageSize,
                string.IsNullOrWhiteSpace(query.Search)
                    ? null
                    : query.Search.Trim(),
                query.Status);

            var products = await _productRepository.GetAllAsync(
                filter,
                cancellationToken);

            var items = products.Items
                .Select(product => new ProductListItem(
                    product.ExternalId,
                    product.Title,
                    product.Sku,
                    product.Price,
                    product.DiscountPrice,
                    product.Stock,
                    product.Condition,
                    product.Status,
                    product.Category,
                    product.Brand,
                    product.PrimaryImageKey is null
                        ? null
                        : _objectStorage.GetPublicUrl(
                            product.PrimaryImageKey),
                    product.CreatedAt))
                .ToArray();

            return Result.Success(
                new PagedResult<ProductListItem>(
                    items,
                    products.Page,
                    products.PageSize,
                    products.TotalItems));
        }
    }
}