namespace Api.Modules.Customer.Application.Abstractions.Authentication
{
    public sealed record AccessTokenResult(
        string Token,
        DateTime ExpiresAtUtc);
    public sealed record RefreshTokenResult(
        string Token,
        DateTime ExpiresAtUtc);
}
