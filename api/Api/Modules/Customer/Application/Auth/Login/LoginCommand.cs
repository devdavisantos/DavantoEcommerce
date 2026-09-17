namespace Api.Modules.Customer.Application.Auth.Login
{
    public sealed record LoginCommand(string Email, string Password, string? Device = null, string? IpAddress = null);
}
