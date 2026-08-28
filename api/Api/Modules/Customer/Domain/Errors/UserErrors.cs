using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidName = new(
            "Customer.User.InvalidName",
            "Name cannot be empty.");

        public static readonly Error InvalidEmail = new(
            "Customer.User.InvalidEmail",
            "Email cannot be empty.");

        public static readonly Error InvalidStripeCustomerId = new(
            "Customer.User.InvalidStripeCustomerId",
            "Stripe Customer ID cannot be empty.");

        public static readonly Error StripeCustomerIdAlreadySet = new(
            "Customer.User.StripeCustomerIdAlreadySet",
            "Stripe Customer ID is already set.");
    }

}
