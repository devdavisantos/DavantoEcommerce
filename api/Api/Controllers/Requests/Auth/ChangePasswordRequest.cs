namespace Api.Controllers.Requests.Auth
{
    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword);
}
