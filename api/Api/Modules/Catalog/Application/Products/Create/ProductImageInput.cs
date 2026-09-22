namespace Api.Modules.Catalog.Application.Products.Create
{
    public sealed record ProductImageInput(
        Stream Content,
        long Length);
}