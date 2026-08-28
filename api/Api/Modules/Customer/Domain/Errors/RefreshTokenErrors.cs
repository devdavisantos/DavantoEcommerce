using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class RefreshTokenErrors
    {
        public static readonly Error InvalidLoginId = new(
            "Custtomers.RefreshToken.InvalidLoginId",
            "Login ID must be valid.");

        public static readonly Error InvalidTokenHash = new(
            "Custtomers.RefreshToken.InvalidTokenHash",
            "Refresh token hash cannot be empty.");

        public static readonly Error InvalidExpiration = new(
            "Custtomers.RefreshToken.InvalidExpiration",
            "Refresh token expiration must be in the future.");

        public static readonly Error AlreadyRevoked = new(
            "Custtomers.RefreshToken.AlreadyRevoked",
            "Refresh token is already revoked.");

        public static readonly Error InvalidToken = new(
            "RefreshToken.InvalidToken",
            "The provided refresh token is invalid.");

        public static readonly Error ExpiredToken = new(
            "RefreshToken.ExpiredToken",
            "The refresh token has expired.");

        public static readonly Error RevokedToken = new(
            "RefreshToken.RevokedToken",
            "The refresh token has been revoked.");

        public static readonly Error ReplacedToken = new(
            "RefreshToken.ReplacedToken",
            "The refresh token has already been used.");
    }
}