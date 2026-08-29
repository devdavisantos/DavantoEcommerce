using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Profile
{
    public static class ProfileErrors
    {
        public static readonly Error UserNotFound = new(
            "Customer.Profile.UserNotFound",
            "The authenticated user was not found.");

        public static readonly Error UserInactive = new(
            "Customer.Profile.UserInactive",
            "The user account is inactive.");
    }
}