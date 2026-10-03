using Api.Shared.Domain;

namespace Api.Modules.Catalog.Application.Products.Errors
{
    public static class ProductApplicationErrors
    {
        public static readonly Error SlugGenerationFailed = new(
            "Catalog.Product.SlugGenerationFailed",
            "A valid slug could not be generated from the product title.");

        public static readonly Error SkuAlreadyExists = new(
            "Catalog.Product.SkuAlreadyExists",
            "The provided SKU is already registered.");

        public static readonly Error SlugConflict = new(
            "Catalog.Product.SlugConflict",
            "A unique product slug could not be generated.");

        public static readonly Error CategoryNotFound = new(
            "Catalog.Product.CategoryNotFound",
            "The selected category was not found.");

        public static readonly Error CategoryInactive = new(
            "Catalog.Product.CategoryInactive",
            "The selected category is inactive.");

        public static readonly Error BrandNotFound = new(
            "Catalog.Product.BrandNotFound",
            "The selected brand was not found.");

        public static readonly Error BrandInactive = new(
            "Catalog.Product.BrandInactive",
            "The selected brand is inactive.");

        public static readonly Error ValidationFailed = new(
            "Catalog.Product.Validation",
            "The product data is invalid.");

        public static readonly Error ImagesTooLarge = new(
            "Catalog.Product.ImagesTooLarge",
            "Product images cannot exceed 100 MB in total.");

        public static readonly Error ImageEmpty = new(
            "Catalog.Product.ImageEmpty",
            "Product image cannot be empty.");

        public static readonly Error ImageTooLarge = new(
            "Catalog.Product.ImageTooLarge",
            "Product image cannot exceed 50 MB.");

        public static readonly Error InvalidImageFormat = new(
            "Catalog.Product.InvalidImageFormat",
            "Product image must be a valid JPEG, PNG or WebP image.");

        public static readonly Error InvalidImageDimensions = new(
            "Catalog.Product.InvalidImageDimensions",
            "Product image dimensions are invalid.");

        public static readonly Error ImageProcessingFailed = new(
            "Catalog.Product.ImageProcessingFailed",
            "Product image could not be processed.");

        public static readonly Error ProductNotFound = new(
            "Catalog.Product.ProductNotFound",
            "The product was not found.");
    }
}