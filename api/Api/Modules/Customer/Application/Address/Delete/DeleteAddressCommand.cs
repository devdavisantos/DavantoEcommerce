namespace Api.Modules.Customer.Application.Address.Delete
{
    public sealed record DeleteAddressCommand(
        Guid UserExternalId,
        Guid AddressExternalId);
}