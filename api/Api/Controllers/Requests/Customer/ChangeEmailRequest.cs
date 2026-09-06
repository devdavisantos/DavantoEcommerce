namespace Api.Controllers.Requests.Customers
{
    public sealed record ChangeEmailRequest(
        string NewEmail);
}