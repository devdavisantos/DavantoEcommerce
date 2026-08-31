namespace Api.Modules.Customer.Application.Address.Update
{
    public sealed record UpdateAddressCommand(
        Guid UserExternalId,
        Guid AddressExternalId,

        bool HasName,
        string? Name,

        bool HasRecipientName,
        string? RecipientName,

        bool HasPostalCode,
        string? PostalCode,

        bool HasStreet,
        string? Street,

        bool HasNumber,
        string? Number,

        bool HasComplement,
        string? Complement,

        bool HasDistrict,
        string? District,

        bool HasCity,
        string? City,

        bool HasState,
        string? State,

        bool HasCountry,
        string? Country);
}