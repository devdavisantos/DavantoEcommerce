namespace Api.Modules.Customer.Application.Profile
{
    public sealed record GetProfileResponse(
        Guid ExternalId,
        string Name,
        string Email,
        string? Document,
        string? Phone);
}
