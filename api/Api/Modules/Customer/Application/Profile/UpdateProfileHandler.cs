using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Profile
{
    public sealed class UpdateProfileHandler
    {
        private readonly IUserRepository _userRepository;

        public UpdateProfileHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> HandleAsync(
            UpdateProfileCommand command,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByExternalIdAsync(
                command.UserExternalId,
                cancellationToken);

            if (user is null)
            {
                return Result.Failure(
                    ProfileErrors.UserNotFound);
            }

            if (user.Status != UserStatus.Active)
            {
                return Result.Failure(
                    ProfileErrors.UserInactive);
            }

            var updateResult = user.UpdateProfile(
                command.HasName,
                command.Name,
                command.HasDocument,
                command.Document,
                command.HasPhone,
                command.Phone);

            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            if (command.HasDocument && !string.IsNullOrWhiteSpace(user.Document))
            {
                var documentExists = await _userRepository.DocumentExistsAsync(
                    user.Document,
                    user.Id,
                    cancellationToken);

                if (documentExists)
                {
                    return Result.Failure(
                        UserErrors.DocumentAlreadyExists);
                }
            }

            await _userRepository.UpdateAsync(
                user,
                cancellationToken);

            return Result.Success();
        }

    }
}
