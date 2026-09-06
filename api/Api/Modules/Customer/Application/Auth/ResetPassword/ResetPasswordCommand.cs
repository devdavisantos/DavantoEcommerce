namespace Api.Modules.Customer.Application.Auth.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Token,
        string NewPassword);
}