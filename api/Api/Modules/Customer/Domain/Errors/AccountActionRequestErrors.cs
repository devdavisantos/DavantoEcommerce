using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class AccountActionRequestErrors
    {
        public static readonly Error InvalidUser = new(
            "Customer.AccountAction.InvalidUser",
            "The user is invalid.");

        public static readonly Error InvalidType = new(
            "Customer.AccountAction.InvalidType",
            "The account action type is invalid.");

        public static readonly Error InvalidToken = new(
            "Customer.AccountAction.InvalidToken",
            "The account action token is invalid.");

        public static readonly Error InvalidExpiration = new(
            "Customer.AccountAction.InvalidExpiration",
            "The account action expiration is invalid.");

        public static readonly Error Expired = new(
            "Customer.AccountAction.Expired",
            "The account action has expired.");

        public static readonly Error Revoked = new(
            "Customer.AccountAction.Revoked",
            "The account action has been revoked.");

        public static readonly Error AlreadyCompleted = new(
            "Customer.AccountAction.AlreadyCompleted",
            "The account action has already been completed.");

        public static readonly Error NotFound = new(
            "Customer.AccountAction.NotFound",
            "The account action was not found.");
    }
}