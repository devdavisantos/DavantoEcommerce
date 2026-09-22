namespace Api.Modules.Admin.Application.Auth.Refresh
{
    public sealed record AdminRefreshCommand(
        string RefreshToken,
        string? Device,
        string? IpAddress);
}