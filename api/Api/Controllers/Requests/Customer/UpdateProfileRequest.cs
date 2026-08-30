using System.Text.Json;

namespace Api.Controllers.Requests.Customer
{
    public sealed record UpdateProfileRequest(
        JsonElement Name,
        JsonElement Document,
        JsonElement Phone);
}
