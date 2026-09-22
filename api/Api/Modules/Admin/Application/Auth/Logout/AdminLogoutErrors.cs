using Api.Shared.Domain;

namespace Api.Modules.Admin.Application.Auth.Logout
{
    public static class AdminLogoutErrors
    {
        public static readonly Error InvalidToken = new(
            "Admin.InvalidRefreshToken",
            "Invalid refresh token.");
    }
}