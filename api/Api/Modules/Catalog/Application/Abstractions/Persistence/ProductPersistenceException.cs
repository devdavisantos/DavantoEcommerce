namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public sealed class ProductPersistenceException : Exception
    {
        public ProductPersistenceFailure Failure { get; }

        public ProductPersistenceException(
            ProductPersistenceFailure failure,
            Exception innerException)
            : base("Product persistence operation failed.", innerException)
        {
            Failure = failure;
        }
    }

    public enum ProductPersistenceFailure
    {
        DuplicateSlug,
        DuplicateSku
    }
}