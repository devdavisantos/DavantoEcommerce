namespace Api.Controllers.Requests.Customers
{
    public sealed record CreateAddressRequest(
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