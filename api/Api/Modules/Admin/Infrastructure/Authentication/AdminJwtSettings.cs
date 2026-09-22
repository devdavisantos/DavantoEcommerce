namespace Api.Modules.Admin.Infrastructure.Authentication
{
    public sealed class AdminJwtSettings
    {
        public const string SectionName = "AdminJwt";

        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; init; }
        public int RefreshTokenExpirationDays { get; init; }
    }
}