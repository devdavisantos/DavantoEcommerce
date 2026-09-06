namespace Api.Modules.Customer.Application.Abstractions.Authentication
{
    public interface IAccountTokenGenerator
    {
        string Generate();

        string Hash(string token);
    }
}