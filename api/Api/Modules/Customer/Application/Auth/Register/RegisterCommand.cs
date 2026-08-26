namespace Api.Modules.Customer.Application.Auth.Register
{
    public sealed record RegisterCommand(
        string Name,
        string Email,
        string Password);
}