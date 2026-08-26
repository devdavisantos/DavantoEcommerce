using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidName = new(
            "Customers.User.InvalidName",
            "Name cannot be empty.");

        public static readonly Error InvalidEmail = new(
            "Customers.User.InvalidEmail",
            "Email cannot be empty.");

        public static readonly Error InvalidStripeCustomerId = new(
            "Customers.User.InvalidStripeCustomerId",
            "Stripe Customer ID cannot be empty.");

        public static readonly Error StripeCustomerIdAlreadySet = new(
            "Customers.User.StripeCustomerIdAlreadySet",
            "Stripe Customer ID is already set.");
    }

}
