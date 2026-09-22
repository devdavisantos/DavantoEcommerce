namespace Api.Modules.Admin.Application.Auth.Login
{
    public sealed record AdminLoginResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt);
}