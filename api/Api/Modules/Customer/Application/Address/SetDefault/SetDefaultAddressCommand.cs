namespace Api.Modules.Customer.Application.Address.SetDefault
{
    public sealed record SetDefaultAddressCommand(
        Guid UserExternalId,
        Guid AddressExternalId);
}