using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Errors
{
    public static class RefreshTokenErrors
    {
        public static readonly Error InvalidLoginId = new(
            "Custtomers.RefreshToken.InvalidLoginId", "Login ID must be valid.");

        public static readonly Error InvalidTokenHash = new(
            "Custtomers.RefreshToken.InvalidTokenHash", "Refresh token hash cannot be empty.");

        public static readonly Error InvalidExpiration = new(
            "Custtomers.RefreshToken.InvalidExpiration", "Refresh token expiration must be in the future.");

        public static readonly Error AlreadyRevoked = new(
            "Custtomers.RefreshToken.AlreadyRevoked", "Refresh token is already revoked.");
    }
}