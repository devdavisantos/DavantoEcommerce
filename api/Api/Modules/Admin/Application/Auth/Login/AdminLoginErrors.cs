using Api.Shared.Domain;

namespace Api.Modules.Admin.Application.Auth.Login
{
    public static class AdminLoginErrors
    {
        public const string ValidationCode = "Admin.Validation";

        public static readonly Error InvalidCredentials = new(
            "Admin.InvalidCredentials",
            "Invalid email or password.");

        public static readonly Error AccountInactive = new(
            "Admin.AccountInactive",
            "Invalid email or password.");

        public static Error Validation(string message) => new(
            ValidationCode,
            message);
    }
}