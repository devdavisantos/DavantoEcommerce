using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;
using System.Net.Mail;

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
                return Result.Failure<User>(
                    UserErrors.InvalidName);
            }

            var nameResult = ValidateAndNormalizeName(name);

            if (nameResult.IsFailure)
            {
                return Result.Failure<User>(
                    UserErrors.InvalidName);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<User>(
                    UserErrors.InvalidEmail);
            }

            if (!IsValidEmail(email))
            {
                return Result.Failure<User>(
                    UserErrors.InvalidEmail);
            }

            var user = new User
            {
                ExternalId = Guid.NewGuid(),
                Name = nameResult.Value,
                Email = email.Trim().ToLowerInvariant(),
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
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Delete()
        {
            if (DeletedAt.HasValue)
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

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim().ToLowerInvariant();

            try
            {
                var address = new MailAddress(email);

                if (!string.Equals(
                    address.Address,
                    email,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var atIndex = email.LastIndexOf("@");

                if (atIndex <= 0 || atIndex == email.Length - 1)
                    return false;

                var domain = email[(atIndex + 1)..];

                if (!domain.Contains('.'))
                {
                    return false;
                }

                if (domain.StartsWith('.') || domain.EndsWith("."))
                {
                    return false;
                }

                var parts = domain.Split('.');

                if (parts.Any(string.IsNullOrWhiteSpace))
                    return false;

                var extension = parts[^1];

                if (extension.Length < 2)
                {
                    return false;
                }

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public Result UpdateProfile(
            bool hasName,
            string? name,
            bool hasDocument,
            string? document,
            bool hasPhone,
            string? phone)
        {
            Result<string> nameResult = null;
            Result<string?>? documentResult = null;
            Result<string?>? phoneResult = null;

            if (hasName)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Result.Failure(
                        UserErrors.InvalidName);
                }

                nameResult =
                    ValidateAndNormalizeName(name);

                if (nameResult.IsFailure)
                {
                    return Result.Failure(
                        nameResult.Error);
                }
            }

            if (hasDocument)
            {
                documentResult =
                    ValidateAndNormalizeDocument(document);

                if (documentResult.IsFailure)
                {
                    return Result.Failure(
                        documentResult.Error);
                }
            }

            if (hasPhone)
            {
                phoneResult =
                    ValidateAndNormalizePhone(phone);

                if (phoneResult.IsFailure)
                {
                    return Result.Failure(
                        phoneResult.Error);
                }
            }

            if (nameResult is not null)
            {
                Name = nameResult.Value;
            }

            if (documentResult is not null)
            {
                Document = documentResult.Value;
            }

            if (phoneResult is not null)
            {
                Phone = phoneResult.Value;
            }

            if (hasName ||
                hasDocument ||
                hasPhone)
            {
                UpdatedAt = DateTime.UtcNow;
            }

            return Result.Success();
        }

        private Result<string?> ValidateAndNormalizeDocument(string? document)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                if (!string.IsNullOrWhiteSpace(Document))
                {
                    return Result.Failure<string?>(
                        UserErrors.DocumentCannotBeRemoved);
                }

                return Result.Success<string?>(null);
            }

            var normalizedDocument = new string(
                document
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                .ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(normalizedDocument))
            {
                return Result.Failure<string?>(
                    UserErrors.InvalidDocument);
            }

            if (normalizedDocument.Length < 4 ||
                normalizedDocument.Length > 50)
            {
                return Result.Failure<string?>(
                    UserErrors.InvalidDocument);
            }

            return Result.Success<string?>(
                normalizedDocument);
        }

        private Result<string?> ValidateAndNormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                if (!string.IsNullOrWhiteSpace(Phone))
                {
                    return Result.Failure<string?>(
                        UserErrors.PhoneCannotBeRemoved);
                }

                return Result.Success<string?>(null);
            }

            if (phone.Any(char.IsLetter))
            {
                return Result.Failure<string?>(
                    UserErrors.PhoneMustContainOnlyNumbers);
            }

            var normalizedPhone = new string(
                phone
                    .Where(char.IsDigit)
                    .ToArray());

            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                return Result.Failure<string?>(
                    UserErrors.InvalidPhone);
            }

            if (normalizedPhone.Length < 7 ||
                normalizedPhone.Length > 15)
            {
                return Result.Failure<string?>(
                    UserErrors.InvalidPhone);
            }

            return Result.Success<string?>(
                normalizedPhone);
        }

        private static Result<string> ValidateAndNormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure<string>(
                    UserErrors.InvalidName);
            }
            
            var normalizedName = string.Join(
                    ' ',
                    name
                        .Trim()
                        .Split(
                            ' ',
                            StringSplitOptions.RemoveEmptyEntries));

            if (normalizedName.Length < 2 ||
                normalizedName.Length > 150)
            {
                return Result.Failure<string>(
                    UserErrors.InvalidName);
            }

            if (!normalizedName.Any(char.IsLetter))
            {
                return Result.Failure<string>(
                    UserErrors.InvalidName);
            }

            foreach (var character in normalizedName)
            {
                if (char.IsLetter(character) ||
                    char.IsWhiteSpace(character) ||
                    character == '\'' ||
                    character == '-')
                {
                    continue;
                }

                return Result.Failure<string>(
                    UserErrors.InvalidName);
            }

            return Result.Success(normalizedName);
        }

        public static Result<string> ValidateAndNormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                !IsValidEmail(email))
            {
                return Result.Failure<string>(
                    UserErrors.InvalidEmail);
            }

            return Result.Success(
                email.Trim().ToLowerInvariant());
        }

        public Result ChangeEmail(string email)
        {
            if (!IsValidEmail(email))
            {
                return Result.Failure(
                    UserErrors.InvalidEmail);
            }

            Email = email
                .Trim()
                .ToLowerInvariant();

            EmailConfirmedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }
    }
}
