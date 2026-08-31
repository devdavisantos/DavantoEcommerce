using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Address.Update
{
    public sealed class UpdateAddressHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;

        public UpdateAddressHandler(
            IUserRepository userRepository,
            IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
        }

        public async Task<Result> HandleAsync(
            UpdateAddressCommand command,
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

            var updateResult = address.Update(
                command.HasName,
                command.Name,
                command.HasRecipientName,
                command.RecipientName,
                command.HasPostalCode,
                command.PostalCode,
                command.HasStreet,
                command.Street,
                command.HasNumber,
                command.Number,
                command.HasComplement,
                command.Complement,
                command.HasDistrict,
                command.District,
                command.HasCity,
                command.City,
                command.HasState,
                command.State,
                command.HasCountry,
                command.Country);

            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            await _addressRepository.UpdateAsync(
                address,
                cancellationToken);

            return Result.Success();
        }
    }
}