using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Profile
{
    public sealed class GetProfileHandler
    {
        private readonly IUserRepository _userRepository;

        public GetProfileHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<GetProfileResponse>> HandleAsync(
            GetProfileQuery query,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByExternalIdAsync(
                query.UserExternalId,
                cancellationToken);

            if (user is null)
            {
                return Result.Failure<GetProfileResponse>(
                    ProfileErrors.UserNotFound);
            }

            var response = new GetProfileResponse(
                user.ExternalId,
                user.Name,
                user.Email,
                user.Document,
                user.Phone);

            return Result.Success(response);
        }
    }
}
