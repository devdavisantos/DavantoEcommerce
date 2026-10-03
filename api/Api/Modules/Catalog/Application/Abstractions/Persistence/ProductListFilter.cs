using Api.Modules.Catalog.Domain.Enums;

namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public sealed record ProductListFilter(
        int Page,
        int PageSize,
        string? Search,
        ProductStatus? Status);
}