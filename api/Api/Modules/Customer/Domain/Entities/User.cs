using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Domain.Entities
{
    public sealed class User
    {
        public ulong Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public string? StripeCustomerId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Document { get; private set; }
        public string? Phone { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime? EmailConfirmedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private User()
        {
        }

        public static Result<User> Create(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure<User>(UserErrors.InvalidName);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<User>(UserErrors.InvalidEmail);
            }

            var user = new User
            {
                ExternalId = Guid.NewGuid(),
                Name = name.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                //Status = UserStatus.PendingVerification,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return Result.Success(user);
        }

        public static User Restore(
            ulong id,
            Guid externalId,
            string? stripeCustomerId,
            string name,
            string email,
            string? document,
            string? phone,
            UserStatus status,
            DateTime? emailConfirmedAt,
            DateTime createdAt,
            DateTime updatedAt,
            DateTime? deletedAt)
        {
            return new User
            {
                Id = id,
                ExternalId = externalId,
                StripeCustomerId = stripeCustomerId,
                Name = name,
                Email = email,
                Document = document,
                Phone = phone,
                Status = status,
                EmailConfirmedAt = emailConfirmedAt,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                DeletedAt = deletedAt
            };
        }

        public Result ConfirmEmail()
        {
            if (EmailConfirmedAt.HasValue)
                return Result.Success();

            EmailConfirmedAt = DateTime.UtcNow;
            //Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Update(string name, string? document, string? phone)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure(UserErrors.InvalidName);
            }

            Name = name.Trim();
            Document = string.IsNullOrWhiteSpace(document) ? null : document.Trim();
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Active()
        {
            Status = UserStatus.Active;
            DeletedAt = null;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Inactive()
        {
            if (Status == UserStatus.Inactive)
                return Result.Success();

            Status = UserStatus.Inactive;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Block()
        {
            if (Status == UserStatus.Blocked)
                return Result.Success();

            Status = UserStatus.Blocked;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result StripeCustomer(string stripeCustomerId)
        {
            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                return Result.Failure(UserErrors.InvalidStripeCustomerId);
            }

            if (!string.IsNullOrWhiteSpace(StripeCustomerId))
            {
                return Result.Failure(UserErrors.StripeCustomerIdAlreadySet);
            }

            StripeCustomerId = stripeCustomerId;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }
    }
}
