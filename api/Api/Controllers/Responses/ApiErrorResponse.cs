namespace Api.Controllers.Responses
{
    public sealed record ApiErrorResponse(
        string Code,
        string Message);
}
