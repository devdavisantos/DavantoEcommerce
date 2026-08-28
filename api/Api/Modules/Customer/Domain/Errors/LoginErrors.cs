using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class LoginErrors
    {
        public static readonly Error InvalidUserId = new(
            "Customer.Login.InvalidUserId",
            "User ID must be valid.");
        
        public static readonly Error InvalidPasswordHash = new(
            "Customer.Login.InvalidPasswordHash",
            "Password hash cannot be empty.");

        public static readonly Error Inactive = new(
            "Customer.Login.Inactive",
            "Inactive login cannot be used.");
    }
}
