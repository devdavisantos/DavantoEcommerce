using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class PasswordResetErrors
    {
        public static readonly Error InvalidEmail = new(
            "Customer.PasswordReset.InvalidEmail",
            "The email address is invalid.");

        public static readonly Error EmailNotFound = new(
            "Customer.PasswordReset.EmailNotFound",
            "No account was found with this email address.");

        public static readonly Error InvalidToken = new(
            "Customer.PasswordReset.InvalidToken",
            "The password reset token is invalid.");

        public static readonly Error ExpiredToken = new(
            "Customer.PasswordReset.ExpiredToken",
            "The password reset token has expired.");

        public static readonly Error RevokedToken = new(
            "Customer.PasswordReset.RevokedToken",
            "The password reset token has been revoked.");

        public static readonly Error AlreadyCompleted = new(
            "Customer.PasswordReset.AlreadyCompleted",
            "The password reset request has already been completed.");

        public static readonly Error UserNotFound = new(
            "Customer.PasswordReset.UserNotFound",
            "The user was not found.");

        public static readonly Error UserInactive = new(
            "Customer.PasswordReset.UserInactive",
            "The user is inactive.");

        public static readonly Error LoginNotFound = new(
            "Customer.PasswordReset.LoginNotFound",
            "An active login was not found.");

        public static readonly Error SamePassword = new(
            "Customer.PasswordReset.SamePassword",
            "The new password must be different from the current password.");

        public static Error Validation(string message) => new(
            "Customer.PasswordReset.Validation",
            message);
    }
}