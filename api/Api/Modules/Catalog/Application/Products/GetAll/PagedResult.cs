namespace Api.Modules.Catalog.Application.Products.GetAll
{
    public sealed record PagedResult<T>(
        IReadOnlyCollection<T> Items,
        int Page,
        int PageSize,
        long TotalItems)
    {
        public int TotalPages =>
            TotalItems == 0
                ? 0
                : (int)Math.Ceiling(
                    TotalItems / (double)PageSize);
    }
}