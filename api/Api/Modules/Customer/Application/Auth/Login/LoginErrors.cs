using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Auth.Login
{
    public static class LoginErrors
    {
        public static readonly Error InvalidCredentials = new(
            "Customer.Login.InvalidCredentials",
            "Invalid email or password.");

        public static readonly Error AccountInactive = new(
            "Customer.Login.AccountInactive",
            "Account is inactive.");

        public static readonly Error AccountBlocked = new(
            "Customer.Login.AccountBlocked",
            "Account is blocked.");

        public static readonly Error EmailNotConfirmed = new(
            "Customer.Login.EmailNotConfirmed",
            "Email has not been confirmed.");

        public static Error Validation(string message)
        {
            return new Error(
                "Customer.Login.Validation",
                message);
        }
    }
}