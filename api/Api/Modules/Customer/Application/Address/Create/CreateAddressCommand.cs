namespace Api.Modules.Customer.Application.Address.Create
{
    public sealed record CreateAddressCommand(
        Guid UserExternalId,
        string? Name,
        string RecipientName,
        string PostalCode,
        string Street,
        string? Number,
        string? Complement,
        string? District,
        string City,
        string State,
        string Country,
        bool IsDefault);
}