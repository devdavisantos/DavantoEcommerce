using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class ChangePasswordErrors
    {
        public static readonly Error InvalidCurrentPassword = new(
            "Customer.ChangePassword.InvalidCurrentPassword",
            "The current password is incorrect.");

        public static readonly Error SamePassword = new(
            "Customer.ChangePassword.SamePassword",
            "The new password must be different from the current password.");

        public static readonly Error UserNotFound = new(
            "Customer.ChangePassword.UserNotFound",
            "The user was not found.");

        public static readonly Error LoginNotFound = new(
            "Customer.ChangePassword.LoginNotFound",
            "An active login was not found.");

        public static readonly Error UserInactive = new(
            "Customer.ChangePassword.UserInactive",
            "The user account is inactive");

        public static Error Validation(string message) => new(
            "Customer.ChangePassword.Validation",
            message);
    }
}
