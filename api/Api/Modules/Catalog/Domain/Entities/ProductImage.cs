using Api.Modules.Catalog.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Catalog.Domain.Entities
{
    public sealed class ProductImage
    {
        public long Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public long ProductId { get; private set; }
        public string ImageKey { get; private set; } = string.Empty;
        public bool IsPrimary { get; private set; }
        public uint SortOrder { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private ProductImage()
        {
        }

        public static Result<ProductImage> Create(
            Guid externalId,
            string imageKey,
            bool isPrimary,
            uint sortOrder)
        {
            if (externalId == Guid.Empty)
            {
                return Result.Failure<ProductImage>(
                    ProductImageErrors.InvalidExternalId);
            }

            if (string.IsNullOrWhiteSpace(imageKey))
            {
                return Result.Failure<ProductImage>(
                    ProductImageErrors.InvalidImageKey);
            }

            var image = new ProductImage
            {
                ExternalId = externalId,
                ImageKey = imageKey.Trim(),
                IsPrimary = isPrimary,
                SortOrder = sortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return Result.Success(image);
        }

        internal void SetAsPrimary()
        {
            IsPrimary = true;
            UpdatedAt = DateTime.UtcNow;
        }

        internal void RemoveAsPrimary()
        {
            IsPrimary = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public Result ChangeSortOrder(uint sortOrder)
        {
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }
    }
}