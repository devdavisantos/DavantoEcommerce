using Api.Modules.Admin.Domain.Entities;

namespace Api.Modules.Admin.Domain.Repositories
{
    public interface IAdminUserRepository
    {
        Task<AdminUser?> GetByIdAsync(int id);
        Task<AdminUser?> GetByExternalIdAsync(Guid externalId);
        Task<AdminUser?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<int> CreateAsync(AdminUser user);
    }
}