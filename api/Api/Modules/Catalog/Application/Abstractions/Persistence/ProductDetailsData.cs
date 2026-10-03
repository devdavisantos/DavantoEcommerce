using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public sealed record ProductDetailsData(
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
        IReadOnlyCollection<ProductImageData> Images,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}