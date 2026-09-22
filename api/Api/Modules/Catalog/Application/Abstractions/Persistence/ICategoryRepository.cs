using Api.Modules.Catalog.Domain.Entities;

namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByExternalIdAsync(
            Guid externalId,
            CancellationToken cancellationToken = default);
    }
}