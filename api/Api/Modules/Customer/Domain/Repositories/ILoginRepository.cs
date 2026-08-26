using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Domain.Repositories
{
    public interface ILoginRepository
    {
        Task<Login?> GetActiveByUserIdAsync(ulong userId, CancellationToken cancellationToken = default);
        Task<Login?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
        Task<ulong> CreateAsync(Login login, CancellationToken cancellationToken = default);
        Task UpdateLastLoginAsync(Login login, CancellationToken cancellationToken = default);
        Task DeactivateAsync(Login login, CancellationToken cancellationToken = default);
    }
}
