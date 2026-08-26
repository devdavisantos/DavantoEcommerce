using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Auth.Register
{
    public static class RegisterErrors
    {
        public static readonly Error EmailAlreadyExists = new(
            "Customers.Register.EmailAlreadyExists",
            "Email is already registered.");

        public static Error Validation(string message)
        {
            return new Error(
                "Customers.Register.Validation",
                message);
        }
    }
}
