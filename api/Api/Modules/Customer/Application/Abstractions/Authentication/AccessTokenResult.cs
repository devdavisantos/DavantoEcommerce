namespace Api.Modules.Customer.Application.Abstractions.Authentication
{
    public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
}
