using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public sealed record ProductListData(
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
        string? PrimaryImageKey,
        DateTime CreatedAt);
}