namespace Api.Modules.Admin.Application.Auth.Refresh
{
    public sealed record AdminRefreshResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt);
}