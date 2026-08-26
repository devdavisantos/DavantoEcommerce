namespace Api.Modules.Customer.Domain.Enums
{
    public enum UserStatus : byte
    {
        PendingVerification = 0,
        Active = 1,
        Inactive = 2,
        Blocked = 3
    }
}
