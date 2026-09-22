namespace Api.Modules.Admin.Application.Profile
{
    public sealed record AdminProfileResponse(
        Guid ExternalId,
        string Name,
        string Email);
}