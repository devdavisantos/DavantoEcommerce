namespace Api.Controllers.Requests.Customer
{
    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword);
}
