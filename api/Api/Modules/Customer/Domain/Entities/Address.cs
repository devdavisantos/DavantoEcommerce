using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Entities
{
    public sealed class Address
    {
        public ulong Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public ulong UserId { get; private set; }
        public string? Name { get; private set; }
        public string RecipientName { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;
        public string Street { get; private set; } = string.Empty;
        public string? Number { get; private set; }
        public string? Complement { get; private set; }
        public string? District { get; private set; }
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public bool Active { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Address()
        {
        }

        public static Result<Address> Create(
            ulong userId,
            string? name,
            string recipientName,
            string postalCode,
            string street,
            string? number,
            string? complement,
            string? district,
            string city,
            string state,
            string country,
            bool isDefault)
        {
            if (userId == 0)
            {
                return Result.Failure<Address>(
                    AddressErrors.InvalidUserId);
            }

            var nameResult =
                ValidateOptional(name, 50, AddressErrors.InvalidName);

            if (nameResult.IsFailure)
            {
                return Result.Failure<Address>(
                    nameResult.Error);
            }

            var recipientResult =
                ValidateRequired(
                    recipientName,
                    150,
                    AddressErrors.InvalidRecipientName);

            if (recipientResult.IsFailure)
            {
                return Result.Failure<Address>(
                    recipientResult.Error);
            }

            var postalCodeResult =
                ValidateRequired(
                    postalCode,
                    20,
                    AddressErrors.InvalidPostalCode);

            if (postalCodeResult.IsFailure)
            {
                return Result.Failure<Address>(
                    postalCodeResult.Error);
            }

            var streetResult =
                ValidateRequired(
                    street,
                    200,
                    AddressErrors.InvalidStreet);

            if (streetResult.IsFailure)
            {
                return Result.Failure<Address>(
                    streetResult.Error);
            }

            var numberResult =
                ValidateOptional(
                    number,
                    30,
                    AddressErrors.InvalidNumber);

            if (numberResult.IsFailure)
            {
                return Result.Failure<Address>(
                    numberResult.Error);
            }

            var complementResult =
                ValidateOptional(
                    complement,
                    150,
                    AddressErrors.InvalidComplement);

            if (complementResult.IsFailure)
            {
                return Result.Failure<Address>(
                    complementResult.Error);
            }

            var districtResult =
                ValidateOptional(
                    district,
                    150,
                    AddressErrors.InvalidDistrict);

            if (districtResult.IsFailure)
            {
                return Result.Failure<Address>(
                    districtResult.Error);
            }

            var cityResult =
                ValidateRequired(
                    city,
                    255,
                    AddressErrors.InvalidCity);

            if (cityResult.IsFailure)
            {
                return Result.Failure<Address>(
                    cityResult.Error);
            }

            var stateResult =
                ValidateRequired(
                    state,
                    255,
                    AddressErrors.InvalidState);

            if (stateResult.IsFailure)
            {
                return Result.Failure<Address>(
                    stateResult.Error);
            }

            var countryResult =
                ValidateRequired(
                    country,
                    255,
                    AddressErrors.InvalidCountry);

            if (countryResult.IsFailure)
            {
                return Result.Failure<Address>(
                    countryResult.Error);
            }

            var now = DateTime.UtcNow;

            var address = new Address
            {
                ExternalId = Guid.NewGuid(),
                UserId = userId,

                Name = nameResult.Value,
                RecipientName = recipientResult.Value,
                PostalCode = postalCodeResult.Value,
                Street = streetResult.Value,
                Number = numberResult.Value,
                Complement = complementResult.Value,
                District = districtResult.Value,
                City = cityResult.Value,
                State = stateResult.Value,
                Country = countryResult.Value,

                Active = true,
                IsDefault = isDefault,

                CreatedAt = now,
                UpdatedAt = now
            };

            return Result.Success(address);
        }

        public static Address Restore(
            ulong id,
            Guid externalId,
            ulong userId,
            string? name,
            string recipientName,
            string postalCode,
            string street,
            string? number,
            string? complement,
            string? district,
            string city,
            string state,
            string country,
            bool active,
            bool isDefault,
            DateTime createdAt,
            DateTime updatedAt)
        {
            return new Address
            {
                Id = id,
                ExternalId = externalId,
                UserId = userId,

                Name = name,
                RecipientName = recipientName,
                PostalCode = postalCode,
                Street = street,
                Number = number,
                Complement = complement,
                District = district,
                City = city,
                State = state,
                Country = country,

                Active = active,
                IsDefault = isDefault,

                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }

        public Result Update(
    bool hasName,
    string? name,
    bool hasRecipientName,
    string? recipientName,
    bool hasPostalCode,
    string? postalCode,
    bool hasStreet,
    string? street,
    bool hasNumber,
    string? number,
    bool hasComplement,
    string? complement,
    bool hasDistrict,
    string? district,
    bool hasCity,
    string? city,
    bool hasState,
    string? state,
    bool hasCountry,
    string? country)
        {
            if (!Active)
            {
                return Result.Failure(
                    AddressErrors.InactiveAddress);
            }

            string? normalizedName = null;
            string? normalizedRecipientName = null;
            string? normalizedPostalCode = null;
            string? normalizedStreet = null;
            string? normalizedNumber = null;
            string? normalizedComplement = null;
            string? normalizedDistrict = null;
            string? normalizedCity = null;
            string? normalizedState = null;
            string? normalizedCountry = null;

            if (hasName)
            {
                var result =
                    ValidateOptional(
                        name,
                        50,
                        AddressErrors.InvalidName);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedName = result.Value;
            }

            if (hasRecipientName)
            {
                var result =
                    ValidateRequired(
                        recipientName,
                        150,
                        AddressErrors.InvalidRecipientName);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedRecipientName = result.Value;
            }

            if (hasPostalCode)
            {
                var result =
                    ValidateRequired(
                        postalCode,
                        20,
                        AddressErrors.InvalidPostalCode);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedPostalCode = result.Value;
            }

            if (hasStreet)
            {
                var result =
                    ValidateRequired(
                        street,
                        200,
                        AddressErrors.InvalidStreet);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedStreet = result.Value;
            }

            if (hasNumber)
            {
                var result =
                    ValidateOptional(
                        number,
                        30,
                        AddressErrors.InvalidNumber);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedNumber = result.Value;
            }

            if (hasComplement)
            {
                var result =
                    ValidateOptional(
                        complement,
                        150,
                        AddressErrors.InvalidComplement);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedComplement = result.Value;
            }

            if (hasDistrict)
            {
                var result =
                    ValidateOptional(
                        district,
                        150,
                        AddressErrors.InvalidDistrict);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedDistrict = result.Value;
            }

            if (hasCity)
            {
                var result =
                    ValidateRequired(
                        city,
                        255,
                        AddressErrors.InvalidCity);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedCity = result.Value;
            }

            if (hasState)
            {
                var result =
                    ValidateRequired(
                        state,
                        255,
                        AddressErrors.InvalidState);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedState = result.Value;
            }

            if (hasCountry)
            {
                var result =
                    ValidateRequired(
                        country,
                        255,
                        AddressErrors.InvalidCountry);

                if (result.IsFailure)
                    return Result.Failure(result.Error);

                normalizedCountry = result.Value;
            }

            if (hasName)
                Name = normalizedName;

            if (hasRecipientName)
                RecipientName = normalizedRecipientName!;

            if (hasPostalCode)
                PostalCode = normalizedPostalCode!;

            if (hasStreet)
                Street = normalizedStreet!;

            if (hasNumber)
                Number = normalizedNumber;

            if (hasComplement)
                Complement = normalizedComplement;

            if (hasDistrict)
                District = normalizedDistrict;

            if (hasCity)
                City = normalizedCity!;

            if (hasState)
                State = normalizedState!;

            if (hasCountry)
                Country = normalizedCountry!;

            if (hasName ||
                hasRecipientName ||
                hasPostalCode ||
                hasStreet ||
                hasNumber ||
                hasComplement ||
                hasDistrict ||
                hasCity ||
                hasState ||
                hasCountry)
            {
                UpdatedAt = DateTime.UtcNow;
            }

            return Result.Success();
        }

        public Result SetAsDefault()
        {
            if (!Active)
            {
                return Result.Failure(
                    AddressErrors.InactiveAddress);
            }

            if (IsDefault)
            {
                return Result.Success();
            }

            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result RemoveAsDefault()
        {
            if (!IsDefault)
            {
                return Result.Success();
            }

            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Inactive()
        {
            if (!Active)
            {
                return Result.Success();
            }

            Active = false;
            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        private static Result<string> ValidateRequired(
            string? value,
            int maxLength,
            Error error)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<string>(
                    error);
            }

            var normalized = Normalize(value);

            if (normalized.Length > maxLength)
            {
                return Result.Failure<string>(
                    error);
            }

            return Result.Success(normalized);
        }

        private static Result<string?> ValidateOptional(
            string? value,
            int maxLength,
            Error error)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Success<string?>(null);
            }

            var normalized = Normalize(value);

            if (normalized.Length > maxLength)
            {
                return Result.Failure<string?>(
                    error);
            }

            return Result.Success<string?>(
                normalized);
        }

        private static string Normalize(
            string value)
        {
            return string.Join(
                ' ',
                value
                    .Trim()
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries));
        }

    }
}
