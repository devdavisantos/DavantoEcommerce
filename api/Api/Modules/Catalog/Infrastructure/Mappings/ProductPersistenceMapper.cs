using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Infrastructure.Mappings
{
    internal static class ProductPersistenceMapper
    {
        public static string ToDatabase(ProductCondition condition)
        {
            return condition switch
            {
                ProductCondition.New => "NEW",
                ProductCondition.Used => "USED",
                ProductCondition.Refurbished => "REFURBISHED",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(condition),
                    condition,
                    "Unsupported product condition.")
            };
        }

        public static string ToDatabase(ProductStatus status)
        {
            return status switch
            {
                ProductStatus.Draft => "DRAFT",
                ProductStatus.Active => "ACTIVE",
                ProductStatus.Discontinued => "DISCONTINUED",
                ProductStatus.Inactive => "INACTIVE",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "Unsupported product status.")
            };
        }

        public static ProductCondition ToProductCondition(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "NEW" => ProductCondition.New,
                "USED" => ProductCondition.Used,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Unsupported product condition.")
            };
        }

        public static ProductStatus ToProductStatus(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "DRAFT" => ProductStatus.Draft,
                "ACTIVE" => ProductStatus.Active,
                "INACTIVE" => ProductStatus.Inactive,
                "DISCONTINUED" => ProductStatus.Discontinued,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Unsupported product status.")
            };
        }
    }
}