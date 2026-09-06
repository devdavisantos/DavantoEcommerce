namespace Api.Modules.Customer.Application.Email.Confirm
{
    public sealed record ConfirmEmailChangeCommand(
        string Token);
}