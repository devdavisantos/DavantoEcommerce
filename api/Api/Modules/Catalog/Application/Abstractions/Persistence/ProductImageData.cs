namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public sealed record ProductImageData(
        Guid ExternalId,
        string ImageKey,
        bool IsPrimary,
        uint SortOrder);
}