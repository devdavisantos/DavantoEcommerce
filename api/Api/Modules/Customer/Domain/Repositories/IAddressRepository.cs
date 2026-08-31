using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Domain.Repositories
{
    public interface IAddressRepository
    {
        Task<IReadOnlyList<Address>> GetActiveByUserIdAsync(
            ulong userId,
            CancellationToken cancellationToken = default);

        Task<Address?> GetByExternalIdAndUserIdAsync(
            Guid externalId,
            ulong userId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveAddressesAsync(
            ulong userId,
            CancellationToken cancellationToken = default);

        Task<ulong> CreateAsync(
            Address address,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Address address,
            CancellationToken cancellationToken = default);

        Task RemoveDefaultByUserIdAsync(
            ulong userId,
            CancellationToken cancellationToken = default);

        Task<Address?> GetFirstActiveByUserIdAsync(
            ulong userId,
            ulong excludeAddressId,
            CancellationToken cancellationToken = default);
    }
}