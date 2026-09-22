namespace Api.Modules.Admin.Application.Auth.Login
{
    public sealed record AdminLoginCommand(
        string Email,
        string Password,
        string? Device,
        string? IpAddress);
}