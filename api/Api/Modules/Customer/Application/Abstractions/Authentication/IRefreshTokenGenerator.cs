namespace Api.Modules.Customer.Application.Abstractions.Authentication
{
    public interface IRefreshTokenGenerator
    {
        RefreshTokenResult Generate();
        string Hash(string token);
    }
}
