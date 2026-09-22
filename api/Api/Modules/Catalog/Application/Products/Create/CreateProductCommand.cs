using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Products.Create
{
    public sealed record CreateProductCommand(
        Guid? CategoryExternalId,
        Guid? BrandExternalId,
        string Title,
        string Description,
        string? Sku,
        decimal Price,
        decimal? DiscountPrice,
        uint Stock,
        ProductCondition Condition,
        IReadOnlyCollection<ProductImageInput> Images);
}