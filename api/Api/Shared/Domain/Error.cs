namespace Api.Shared.Domain
{
    public sealed record Error (string Code, string Message)
    {
        public static readonly Error None = new Error(string.Empty, string.Empty);
    }
}
