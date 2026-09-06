using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Domain.Repositories
{
    public interface IAccountActionRequestRepository
    {
        Task<AccountActionRequest?> GetByTokenHashAsync(
            string tokenHash,
            string type,
            CancellationToken cancellationToken = default);

        Task<ulong> CreateAsync(
            AccountActionRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            AccountActionRequest request,
            CancellationToken cancellationToken = default);

        Task RevokePendingByUserIdAndTypeAsync(
            ulong userId,
            string type,
            CancellationToken cancellationToken = default);
    }
}