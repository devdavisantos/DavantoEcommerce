using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Products.GetById
{
    public sealed record ProductDetails(
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
        ProductCondition Condition,
        ProductStatus Status,
        IReadOnlyCollection<ProductImageDetails> Images,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}