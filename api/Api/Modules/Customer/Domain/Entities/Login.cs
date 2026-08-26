using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Entities
{
    public sealed class Login
    {
        public ulong Id { get; private set; }
        public ulong UserId { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public bool Active { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Login()
        {
        }

        public static Result<Login> Create(ulong userId, string passwordHash)
        {
            if (userId == 0)
            {
                return Result.Failure<Login>(
                    LoginErrors.InvalidUserId);
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return Result.Failure<Login>(
                    LoginErrors.InvalidPasswordHash);
            }

            var login = new Login
            {
                UserId = userId,
                PasswordHash = passwordHash,
                Active = true,
                LastLoginAt = null,
                CreatedAt = DateTime.UtcNow
            };

            return Result.Success(login);
        }

        public static Login Restore(
            ulong id,
            ulong userId,
            string passwordHash,
            bool active,
            DateTime? lastLoginAt,
            DateTime createdAt)
        {
            return new Login
            {
                Id = id,
                UserId = userId,
                PasswordHash = passwordHash,
                Active = active,
                LastLoginAt = lastLoginAt,
                CreatedAt = createdAt
            };
        }

        public Result RegisterSuccessfulLogin()
        {
            if (!Active)
            {
                return Result.Failure(
                    LoginErrors.Inactive);
            }
            
            LastLoginAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!Active)
                return Result.Success();

            Active = false;

            return Result.Success();
        }

    }
}
