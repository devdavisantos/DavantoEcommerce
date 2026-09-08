using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Address.Delete
{
    public sealed class DeleteAddressHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IDbSession _dbSession;

        public DeleteAddressHandler(
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _dbSession = dbSession;
        }

        public async Task<Result> HandleAsync(
            DeleteAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            var user =
                await _userRepository.GetByExternalIdAsync(
                    command.UserExternalId,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure(
                    AddressErrors.InvalidUserId);
            }

            var address =
                await _addressRepository
                    .GetByExternalIdAndUserIdAsync(
                        command.AddressExternalId,
                        user.Id,
                        cancellationToken);

            if (address is null)
            {
                return Result.Failure(
                    AddressErrors.NotFound);
            }

            if (address.Status != AddressStatus.Active)
            {
                return Result.Failure(
                    AddressErrors.InactiveAddress);
            }

            var wasDefault =
                address.IsDefault;

            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var inactiveResult =
                    address.Inactive();

                if (inactiveResult.IsFailure)
                {
                    await _dbSession.RollbackAsync(
                        cancellationToken);

                    return inactiveResult;
                }

                await _addressRepository.UpdateAsync(
                    address,
                    cancellationToken);

                if (wasDefault)
                {
                    var nextAddress =
                        await _addressRepository
                            .GetFirstActiveByUserIdAsync(
                                user.Id,
                                address.Id,
                                cancellationToken);

                    if (nextAddress is not null)
                    {
                        var defaultResult =
                            nextAddress.SetAsDefault();

                        if (defaultResult.IsFailure)
                        {
                            await _dbSession.RollbackAsync(
                                cancellationToken);

                            return defaultResult;
                        }

                        await _addressRepository.UpdateAsync(
                            nextAddress,
                            cancellationToken);
                    }
                }

                await _dbSession.CommitAsync(
                    cancellationToken);

                return Result.Success();
            }
            catch
            {
                await _dbSession.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}