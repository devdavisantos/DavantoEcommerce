namespace Api.Modules.Customer.Application.Email.Change
{
    public sealed record ChangeEmailCommand(
        Guid UserExternalId,
        string NewEmail);
}