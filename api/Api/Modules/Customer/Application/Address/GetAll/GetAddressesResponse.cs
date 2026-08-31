namespace Api.Modules.Customer.Application.Address.GetAll
{
    public sealed record GetAddressesResponse(
        Guid ExternalId,
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