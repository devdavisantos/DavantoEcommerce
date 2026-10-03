using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Products.GetAll
{
    public sealed record ProductListItem(
        Guid ExternalId,
        string Title,
        string? Sku,
        decimal Price,
        decimal? DiscountPrice,
        uint Stock,
        ProductCondition Condition,
        ProductStatus Status,
        string? Category,
        string? Brand,
        string? PrimaryImageUrl,
        DateTime CreatedAt);
}