using Api.Shared.Domain;

namespace Api.Modules.Admin.Application.Auth.Refresh
{
    public static class AdminRefreshErrors
    {
        public static readonly Error InvalidToken = new(
            "Admin.RefreshToken.InvalidToken",
            "Invalid refresh token.");

        public static readonly Error ExpiredToken = new(
            "Admin.RefreshToken.ExpiredToken",
            "Refresh token has expired.");

        public static readonly Error RevokedToken = new(
            "Admin.RefreshToken.RevokedToken",
            "Refresh token has been revoked.");

        public static readonly Error ReplacedToken = new(
            "Admin.RefreshToken.ReplacedToken",
            "Refresh token has already been replaced.");

        public static readonly Error InvalidSession = new(
            "Admin.RefreshToken.InvalidSession",
            "Invalid admin session.");
    }
}