using Api.Modules.Catalog.Domain.Enums;
using Api.Modules.Catalog.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Catalog.Domain.Entities
{
    public sealed class Product
    {
        private readonly List<ProductImage> _images = [];

        public long Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public uint? CategoryId { get; private set; }
        public uint? BrandId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? Sku { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public decimal? DiscountPrice { get; private set; }
        public uint Stock { get; private set; }
        public ProductCondition Condition { get; private set; }
        public ProductStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        private Product()
        {
        }

        public static Result<Product> Create(
            uint? categoryId,
            uint? brandId,
            string title,
            string slug,
            string description,
            string? sku,
            decimal price,
            decimal? discountPrice,
            uint stock,
            ProductCondition condition)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return Result.Failure<Product>(
                    ProductErrors.InvalidTitle);
            }

            if (string.IsNullOrWhiteSpace(slug))
            {
                return Result.Failure<Product>(
                    ProductErrors.InvalidSlug);
            }

            if (price < 0)
            {
                return Result.Failure<Product>(
                    ProductErrors.InvalidPrice);
            }

            if (discountPrice is < 0 ||
                discountPrice >= price)
            {
                return Result.Failure<Product>(
                    ProductErrors.InvalidDiscountPrice);
            }

            var product = new Product
            {
                ExternalId = Guid.NewGuid(),
                CategoryId = categoryId,
                BrandId = brandId,
                Title = title.Trim(),
                Slug = slug.Trim(),
                Description = description.Trim(),
                Sku = string.IsNullOrWhiteSpace(sku)
                    ? null
                    : sku.Trim().ToUpperInvariant(),
                Price = price,
                DiscountPrice = discountPrice,
                Stock = stock,
                Condition = condition,
                Status = ProductStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return Result.Success(product);
        }

        public Result AddImage(ProductImage image)
        {
            if (_images.Any(x => x.ExternalId == image.ExternalId))
            {
                return Result.Failure(
                    ProductErrors.ImageAlreadyExists);
            }

            if (image.IsPrimary)
            {
                foreach (var currentImage in _images)
                    currentImage.RemoveAsPrimary();
            }

            _images.Add(image);
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result SetPrimaryImage(Guid imageExternalId)
        {
            var image = _images.FirstOrDefault(
                x => x.ExternalId == imageExternalId);

            if (image is null)
            {
                return Result.Failure(
                    ProductErrors.ImageNotFound);
            }

            foreach (var currentImage in _images)
            {
                if (currentImage.ExternalId == imageExternalId)
                    currentImage.SetAsPrimary();
                else
                    currentImage.RemoveAsPrimary();
            }

            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result RemoveImage(Guid imageExternalId)
        {
            var image = _images.FirstOrDefault(
                x => x.ExternalId == imageExternalId);

            if (image is null)
            {
                return Result.Failure(
                    ProductErrors.ImageNotFound);
            }

            var wasPrimary = image.IsPrimary;

            _images.Remove(image);

            if (wasPrimary && _images.Count > 0)
            {
                _images
                    .OrderBy(x => x.SortOrder)
                    .First()
                    .SetAsPrimary();
            }

            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result ChangePrice(
            decimal price,
            decimal? discountPrice)
        {
            if (price < 0)
            {
                return Result.Failure(
                    ProductErrors.InvalidPrice);
            }

            if (discountPrice is < 0 ||
                discountPrice >= price)
            {
                return Result.Failure(
                    ProductErrors.InvalidDiscountPrice);
            }

            Price = price;
            DiscountPrice = discountPrice;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result ChangeStock(uint stock)
        {
            Stock = stock;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Activate()
        {
            if (_images.Count == 0)
            {
                return Result.Failure(
                    ProductErrors.ImageRequired);
            }

            if (!_images.Any(x => x.IsPrimary))
            {
                return Result.Failure(
                    ProductErrors.PrimaryImageRequired);
            }

            Status = ProductStatus.Active;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Deactivate()
        {
            Status = ProductStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Discontinue()
        {
            Status = ProductStatus.Discontinued;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }
    }
}