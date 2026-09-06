using Api.Modules.Customer.Domain.Entities;

namespace Api.Modules.Customer.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(ulong userId, CancellationToken cancellationToken = default);
        Task<User?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, ulong excludeUserId, CancellationToken cancellationToken = default);
        Task<ulong> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> DocumentExistsAsync(string document, ulong excludeUserId, CancellationToken cancellationToken = default);
    }
}