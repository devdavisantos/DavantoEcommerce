using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class RefreshTokenErrors
    {
        public static readonly Error InvalidLoginId = new(
            "Customers.RefreshToken.InvalidLoginId",
            "Login ID must be valid.");

        public static readonly Error InvalidTokenHash = new(
            "Customers.RefreshToken.InvalidTokenHash",
            "Refresh token hash cannot be empty.");

        public static readonly Error InvalidExpiration = new(
            "Customers.RefreshToken.InvalidExpiration",
            "Refresh token expiration must be in the future.");

        public static readonly Error AlreadyRevoked = new(
            "Customers.RefreshToken.AlreadyRevoked",
            "Refresh token is already revoked.");

        public static readonly Error InvalidToken = new(
            "Customer.RefreshToken.InvalidToken",
            "The provided refresh token is invalid.");

        public static readonly Error ExpiredToken = new(
            "Customer.RefreshToken.ExpiredToken",
            "The refresh token has expired.");

        public static readonly Error RevokedToken = new(
            "Customer.RefreshToken.RevokedToken",
            "The refresh token has been revoked.");

        public static readonly Error ReplacedToken = new(
            "Customer.RefreshToken.ReplacedToken",
            "The refresh token has already been used.");

        public static readonly Error InactiveLogin = new(
            "Customer.RefreshToken.InactiveLogin",
            "The login associated with this refresh token is inactive.");

        public static readonly Error UserInactive = new(
            "Customer.RefreshToken.UserInactive",
            "The user associated with this refresh token is inactive.");

        public static readonly Error UserBlocked = new(
            "Customer.RefreshToken.UserBlocked",
            "The user associated with this refresh token is blocked.");
    }
}