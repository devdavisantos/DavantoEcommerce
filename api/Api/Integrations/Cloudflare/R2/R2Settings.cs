namespace Api.Integrations.Cloudflare.R2;

public sealed class R2Settings
{
    public const string SectionName = "R2";
    public string AccountId { get; init; } = string.Empty;
    public string AccessKeyId { get; init; } = string.Empty;
    public string SecretAccessKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public string PublicBaseUrl { get; init; } = string.Empty;
}