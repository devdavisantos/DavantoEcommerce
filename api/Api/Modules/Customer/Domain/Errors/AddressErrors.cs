using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class AddressErrors
    {
        public static readonly Error InvalidUserId = new(
            "Customer.Address.InvalidUserId",
            "The user id invalid");

        public static readonly Error InvalidName = new(
            "Customer.Address.InvalidName",
            "The address name is invalid.");

        public static readonly Error InvalidRecipientName = new(
            "Customer.Address.InvalidRecipientName",
            "The recipient name is invalid.");

        public static readonly Error InvalidPostalCode = new(
            "Customer.Address.InvalidPostalCode",
            "The postal code is invalid.");

        public static readonly Error InvalidStreet = new(
            "Customer.Address.InvalidStreet",
            "The street is invalid.");

        public static readonly Error InvalidNumber = new(
            "Customer.Address.InvalidNumber",
            "The address number is invalid.");

        public static readonly Error InvalidComplement = new(
            "Customer.Address.InvalidComplement",
            "The complement is invalid.");

        public static readonly Error InvalidDistrict = new(
            "Customer.Address.InvalidDistrict",
            "The district is invalid.");

        public static readonly Error InvalidCity = new(
            "Customer.Address.InvalidCity",
            "The city is invalid.");

        public static readonly Error InvalidState = new(
            "Customer.Address.InvalidState",
            "The state is invalid.");

        public static readonly Error InvalidCountry = new(
            "Customer.Address.InvalidCountry",
            "The country is invalid.");

        public static readonly Error NotFound = new(
            "Customer.Address.NotFound",
            "The address was not found.");

        public static readonly Error InactiveAddress = new(
            "Customer.Address.Inactive",
            "The address is inactive.");
    }
}
