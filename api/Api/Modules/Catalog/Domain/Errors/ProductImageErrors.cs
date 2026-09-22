using Api.Shared.Domain;

namespace Api.Modules.Catalog.Domain.Errors
{
    public static class ProductImageErrors
    {
        public static readonly Error InvalidImageKey = new(
            "Catalog.ProductImage.InvalidImageKey",
            "Product image key cannot be empty.");

        public static readonly Error InvalidExternalId = new(
            "Catalog.ProductImage.InvalidExternalId",
            "Product image external ID is invalid.");
    }
}