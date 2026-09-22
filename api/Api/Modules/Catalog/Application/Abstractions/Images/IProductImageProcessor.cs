using Api.Modules.Catalog.Application.Products.Create;
using Api.Shared.Domain;

namespace Api.Modules.Catalog.Application.Abstractions.Images
{
    public interface IProductImageProcessor
    {
        Task<Result<ProcessedProductImage>> ProcessAsync(
            ProductImageInput image,
            CancellationToken cancellationToken = default);
    }
}