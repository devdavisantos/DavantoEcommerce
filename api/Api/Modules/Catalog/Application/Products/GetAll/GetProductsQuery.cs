using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Products.GetAll
{
    public sealed record GetProductsQuery(
        int Page = 1,
        int PageSize = 20,
        string? Search = null,
        ProductStatus? Status = null);
}