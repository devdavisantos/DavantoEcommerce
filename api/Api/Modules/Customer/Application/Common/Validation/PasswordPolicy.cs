namespace Api.Modules.Customer.Application.Common.Validation
{
    public static class PasswordPolicy
    {
        public const int MinimumLength = 8;
        public const int MaximumLength = 128;
        public const string RequiredMessage = "Password is required.";
        public const string MinimumLengthMessage = $"Password must contain at least 8 characters.";
        public const string MaximumLengthMessage = $"Password cannot exceed 128 characters.";
    }
}
