namespace Api.Modules.Customer.Application.Auth.Refresh
{
    public sealed record RefreshResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken);
}
