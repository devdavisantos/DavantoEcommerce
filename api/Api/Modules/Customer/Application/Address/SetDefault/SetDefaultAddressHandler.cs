using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Address.SetDefault
{
    public sealed class SetDefaultAddressHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IDbSession _dbSession;

        public SetDefaultAddressHandler(
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _dbSession = dbSession;
        }

        public async Task<Result> HandleAsync(
            SetDefaultAddressCommand command,
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

            if (!address.Active)
            {
                return Result.Failure(
                    AddressErrors.InactiveAddress);
            }

            if (address.IsDefault)
            {
                return Result.Success();
            }

            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                await _addressRepository
                    .RemoveDefaultByUserIdAsync(
                        user.Id,
                        cancellationToken);

                var result =
                    address.SetAsDefault();

                if (result.IsFailure)
                {
                    await _dbSession.RollbackAsync(
                        cancellationToken);

                    return result;
                }

                await _addressRepository.UpdateAsync(
                    address,
                    cancellationToken);

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