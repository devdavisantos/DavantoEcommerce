namespace Api.Controllers.Requests.Customers
{
    public sealed record ConfirmEmailChangeRequest(
        string Token);
}