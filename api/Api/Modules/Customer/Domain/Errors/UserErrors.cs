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
            "Email email address is invalid.");

        public static readonly Error InvalidDocument = new(
            "Customer.User.InvalidDocument",
            "The document must contain at least 4 letters of numbers.");

        public static readonly Error DocumentAlreadyExists = new(
            "Custotmer.User.DocumentAlreadyExists",
            "The document is already registered.");

        public static readonly Error InvalidPhone = new(
            "Customer.User.InvalidPhone",
            "The phone number must contain at least 7 digits.");

        public static readonly Error InvalidStripeCustomerId = new(
            "Customer.User.InvalidStripeCustomerId",
            "Stripe Customer ID cannot be empty.");

        public static readonly Error StripeCustomerIdAlreadySet = new(
            "Customer.User.StripeCustomerIdAlreadySet",
            "Stripe Customer ID is already set.");

        public static readonly Error DocumentCannotBeRemoved = new(
            "Customer.User.DocumentCannotBeRemoved",
            "The document cannot be removed once it has been provided.");

        public static readonly Error PhoneCannotBeRemoved = new(
            "Customer.User.PhoneCannotBeRemoved",
            "The phone cannot be removed once it has been provided.");

        public static readonly Error PhoneMustContainOnlyNumbers = new(
            "Customer.User.PhoneMustContainOnlyNumbers",
            "The phone number cannot contain letters.");
    }

}
