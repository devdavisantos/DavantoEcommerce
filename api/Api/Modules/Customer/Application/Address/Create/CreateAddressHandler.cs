using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;
using AddressEntity = Api.Modules.Customer.Domain.Entities.Address;

namespace Api.Modules.Customer.Application.Address.Create
{
    public sealed class CreateAddressHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IDbSession _dbSession;

        public CreateAddressHandler(
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _dbSession = dbSession;
        }

        public async Task<Result<CreateAddressResponse>>
            HandleAsync(
                CreateAddressCommand command,
                CancellationToken cancellationToken = default)
        {
            var user =
                await _userRepository.GetByExternalIdAsync(
                    command.UserExternalId,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure<CreateAddressResponse>(
                    AddressErrors.InvalidUserId);
            }

            var hasAddresses =
                await _addressRepository
                    .HasActiveAddressesAsync(
                        user.Id,
                        cancellationToken);

            var isDefault =
                !hasAddresses || command.IsDefault;

            var addressResult = AddressEntity.Create(
                user.Id,
                command.Name,
                command.RecipientName,
                command.PostalCode,
                command.Street,
                command.Number,
                command.Complement,
                command.District,
                command.City,
                command.State,
                command.Country,
                isDefault);

            if (addressResult.IsFailure)
            {
                return Result.Failure<CreateAddressResponse>(
                    addressResult.Error);
            }

            var address = addressResult.Value;

            if (address.IsDefault && hasAddresses)
            {
                await _addressRepository
                    .RemoveDefaultByUserIdAsync(
                        user.Id,
                        cancellationToken);
            }

            await _addressRepository.CreateAsync(
                address,
                cancellationToken);

            return Result.Success(
                new CreateAddressResponse(
                    address.ExternalId));
        }
    }
}