using Api.Modules.Catalog.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers.Requests.Admin.Products
{
    public sealed class CreateProductRequest
    {
        public Guid? CategoryExternalId { get; init; }
        public Guid? BrandExternalId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public decimal Price { get; init; }
        public decimal? DiscountPrice { get; init; }
        public uint Stock { get; init; }
        public ProductCondition Condition { get; init; }
        public List<IFormFile> Images { get; init; } = [];
    }
}