namespace Api.Controllers.Requests.Auth
{
    public sealed record ResetPasswordRequest(
        string Token,
        string NewPassword);
}