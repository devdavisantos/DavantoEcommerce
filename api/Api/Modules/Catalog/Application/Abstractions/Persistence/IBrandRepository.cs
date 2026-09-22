using Api.Modules.Catalog.Domain.Entities;

namespace Api.Modules.Catalog.Application.Abstractions.Persistence
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByExternalIdAsync(
            Guid externalId,
            CancellationToken cancellationToken = default);
    }
}