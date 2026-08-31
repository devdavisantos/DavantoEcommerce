using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Address.GetAll
{
    public sealed class GetAddressesHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;

        public GetAddressesHandler(
            IUserRepository userRepository,
            IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
        }

        public async Task<Result<IReadOnlyList<GetAddressesResponse>>>
            HandleAsync(
                GetAddressesQuery query,
                CancellationToken cancellationToken = default)
        {
            var user =
                await _userRepository.GetByExternalIdAsync(
                    query.UserExternalId,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure<
                    IReadOnlyList<GetAddressesResponse>>(
                    AddressErrors.InvalidUserId);
            }

            var addresses =
                await _addressRepository
                    .GetActiveByUserIdAsync(
                        user.Id,
                        cancellationToken);

            var response = addresses
                .Select(address =>
                    new GetAddressesResponse(
                        address.ExternalId,
                        address.Name,
                        address.RecipientName,
                        address.PostalCode,
                        address.Street,
                        address.Number,
                        address.Complement,
                        address.District,
                        address.City,
                        address.State,
                        address.Country,
                        address.IsDefault))
                .ToList();

            return Result.Success<
                IReadOnlyList<GetAddressesResponse>>(
                response);
        }
    }
}