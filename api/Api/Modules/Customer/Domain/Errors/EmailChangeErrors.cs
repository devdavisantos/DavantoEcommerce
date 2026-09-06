using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class EmailChangeErrors
    {
        public static readonly Error UserNotFound = new(
            "Customer.EmailChange.UserNotFound",
            "The user was not found.");

        public static readonly Error UserInactive = new(
            "Customer.EmailChange.UserInactive",
            "The user is inactive.");

        public static readonly Error InvalidEmail = new(
            "Customer.EmailChange.InvalidEmail",
            "The email address is invalid.");

        public static readonly Error SameEmail = new(
            "Customer.EmailChange.SameEmail",
            "The new email address must be different from the current email address.");

        public static readonly Error EmailAlreadyExists = new(
            "Customer.EmailChange.EmailAlreadyExists",
            "The email address is already registered.");

        public static readonly Error InvalidToken = new(
            "Customer.EmailChange.InvalidToken",
            "The email change token is invalid.");

        public static readonly Error ExpiredToken = new(
            "Customer.EmailChange.ExpiredToken",
            "The email change token has expired.");

        public static readonly Error RevokedToken = new(
            "Customer.EmailChange.RevokedToken",
            "The email change token has been revoked.");

        public static readonly Error AlreadyConfirmed = new(
            "Customer.EmailChange.AlreadyConfirmed",
            "The email change request has already been confirmed.");

        public static readonly Error InvalidMetadata = new(
            "Customer.EmailChange.InvalidMetadata",
            "The email change request metadata is invalid.");
    }
}