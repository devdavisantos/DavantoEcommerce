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

        public static ProductCondition ToCondition(string value)
        {
            return value switch
            {
                "NEW" => ProductCondition.New,
                "USED" => ProductCondition.Used,
                "REFURBISHED" => ProductCondition.Refurbished,
                _ => throw new InvalidOperationException(
                    $"Unsupported product condition '{value}'.")
            };
        }

        public static ProductStatus ToStatus(string value)
        {
            return value switch
            {
                "DRAFT" => ProductStatus.Draft,
                "ACTIVE" => ProductStatus.Active,
                "DISCONTINUED" => ProductStatus.Discontinued,
                "INACTIVE" => ProductStatus.Inactive,
                _ => throw new InvalidOperationException(
                    $"Unsupported product status '{value}'.")
            };
        }
    }
}