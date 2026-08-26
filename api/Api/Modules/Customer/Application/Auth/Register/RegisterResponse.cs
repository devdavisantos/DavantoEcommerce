namespace Api.Modules.Customer.Application.Auth.Register
{
    public sealed record RegisterResponse(
    Guid ExternalId,
    string Name,
    string Email);
}