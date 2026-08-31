using System.Text.Json;

namespace Api.Controllers.Requests.Customers
{
    public sealed record UpdateAddressRequest(
        JsonElement Name,
        JsonElement RecipientName,
        JsonElement PostalCode,
        JsonElement Street,
        JsonElement Number,
        JsonElement Complement,
        JsonElement District,
        JsonElement City,
        JsonElement State,
        JsonElement Country);
}