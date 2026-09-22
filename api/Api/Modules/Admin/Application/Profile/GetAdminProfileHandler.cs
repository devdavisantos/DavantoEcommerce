using Api.Modules.Admin.Domain.Repositories;

namespace Api.Modules.Admin.Application.Profile
{
    public sealed class GetAdminProfileHandler
    {
        private readonly IAdminUserRepository _userRepository;

        public GetAdminProfileHandler(
            IAdminUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<AdminProfileResponse?> HandleAsync(
            Guid externalId)
        {
            var user = await _userRepository.GetByExternalIdAsync(
                externalId);

            if (user is null)
                return null;

            return new AdminProfileResponse(
                user.ExternalId,
                user.Name,
                user.Email);
        }
    }
}