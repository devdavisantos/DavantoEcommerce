namespace Api.Modules.Customer.Application.Email.Change
{
    public sealed record ChangeEmailMetadata(
        string OldEmail,
        string NewEmail);
}