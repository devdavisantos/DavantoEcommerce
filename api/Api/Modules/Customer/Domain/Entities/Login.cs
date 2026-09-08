using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Entities
{
    public sealed class Login
    {
        public ulong Id { get; private set; }
        public ulong UserId { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public LoginStatus Status { get; private set; }
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
                Status = LoginStatus.Active,
                LastLoginAt = null,
                CreatedAt = DateTime.UtcNow
            };

            return Result.Success(login);
        }

        public static Login Restore(
            ulong id,
            ulong userId,
            string passwordHash,
            LoginStatus status,
            DateTime? lastLoginAt,
            DateTime createdAt)
        {
            return new Login
            {
                Id = id,
                UserId = userId,
                PasswordHash = passwordHash,
                Status = status,
                LastLoginAt = lastLoginAt,
                CreatedAt = createdAt
            };
        }

        public Result RegisterSuccessfulLogin()
        {
            if (Status != LoginStatus.Active)
            {
                return Result.Failure(
                    LoginErrors.Inactive);
            }
            
            LastLoginAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (Status != LoginStatus.Active)
                return Result.Success();

            Status = LoginStatus.Inactive;

            return Result.Success();
        }

        public void RegisterLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

    }
}
