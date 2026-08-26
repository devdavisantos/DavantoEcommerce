namespace Api.Controllers.Requests.Auth
{
    public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password);
}
