namespace Api.Modules.Catalog.Application.Products.GetById
{
    public sealed record ProductImageDetails(
        Guid ExternalId,
        string ImageUrl,
        bool IsPrimary,
        uint SortOrder);
}