using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Profile
{
    public static class ProfileErrors
    {
        public static readonly Error UserNotFound = new(
            "Customers.Profile.UserNotFound",
            "The authenticated user was not found.");
    }
}