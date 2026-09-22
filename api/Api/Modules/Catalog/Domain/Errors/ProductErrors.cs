using Api.Shared.Domain;

namespace Api.Modules.Catalog.Domain.Errors
{
    public static class ProductErrors
    {
        public static readonly Error InvalidTitle = new(
            "Catalog.Product.InvalidTitle",
            "Product title cannot be empty.");

        public static readonly Error InvalidSlug = new(
            "Catalog.Product.InvalidSlug",
            "Product slug cannot be empty.");

        public static readonly Error InvalidSku = new(
            "Catalog.Product.InvalidSku",
            "Product SKU cannot be empty.");

        public static readonly Error InvalidPrice = new(
            "Catalog.Product.InvalidPrice",
            "Product price cannot be negative.");

        public static readonly Error InvalidDiscountPrice = new(
            "Catalog.Product.InvalidDiscountPrice",
            "Discount price must be greater than or equal to zero and lower than the product price.");

        public static readonly Error ImageAlreadyExists = new(
            "Catalog.Product.ImageAlreadyExists",
            "The product image is already associated with this product.");

        public static readonly Error ImageNotFound = new(
            "Catalog.Product.ImageNotFound",
            "The product image was not found.");

        public static readonly Error ImageRequired = new(
            "Catalog.Product.ImageRequired",
            "Product must have at least one image before activation.");

        public static readonly Error PrimaryImageRequired = new(
            "Catalog.Product.PrimaryImageRequired",
            "Product must have a primary image before activation.");
    }
}