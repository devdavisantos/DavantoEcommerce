namespace Api.Modules.Catalog.Application.Products.Create
{
    public sealed record ProcessedProductImage(
        Stream Content,
        long Length);
}