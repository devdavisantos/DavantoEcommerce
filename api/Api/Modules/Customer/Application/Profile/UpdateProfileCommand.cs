namespace Api.Modules.Customer.Application.Profile
{
    public sealed record UpdateProfileCommand(
        Guid UserExternalId,
        bool HasName,
        string? Name,
        bool HasDocument,
        string? Document,
        bool HasPhone,
        string? Phone);
}
