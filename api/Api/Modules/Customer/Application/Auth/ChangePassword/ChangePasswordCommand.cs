namespace Api.Modules.Customer.Application.Auth.ChangePassword
{
    public sealed record ChangePasswordCommand(
        Guid UserExternalId,
        string CurrentPassword,
        string NewPassword);
}
