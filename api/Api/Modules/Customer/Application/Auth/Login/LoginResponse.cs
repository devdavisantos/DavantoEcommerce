namespace Api.Modules.Customer.Application.Auth.Login
{
    public sealed record LoginResponse(string AccessToken, DateTime AccessTokenExpiresAt);
}