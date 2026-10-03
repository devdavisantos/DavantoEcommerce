using Api.Modules.Catalog.Application.Products.GetAll;
using Api.Modules.Catalog.Domain.Entities;

namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public interface IProductRepository
    {
        Task<IReadOnlyCollection<string>> GetSlugsByPrefixAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<bool> SkuExistsAsync(
            string sku,
            CancellationToken cancellationToken = default);

        Task CreateAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task<PagedResult<ProductListData>> GetAllAsync(
            ProductListFilter filter,
            CancellationToken cancellationToken = default);

        Task<ProductDetailsData?> GetByExternalIdAsync(
            Guid externalId,
            CancellationToken cancellationToken = default);
    }
}