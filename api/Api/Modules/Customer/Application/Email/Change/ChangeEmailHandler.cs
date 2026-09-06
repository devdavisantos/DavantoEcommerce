using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Domain.Constants;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;
using System.Net.Mail;
using System.Text.Json;

namespace Api.Modules.Customer.Application.Email.Change
{
    public sealed class ChangeEmailHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountActionRequestRepository
            _accountActionRequestRepository;
        private readonly IAccountTokenGenerator
            _accountTokenGenerator;
        private readonly IDbSession _dbSession;

        public ChangeEmailHandler(
            IUserRepository userRepository,
            IAccountActionRequestRepository
                accountActionRequestRepository,
            IAccountTokenGenerator accountTokenGenerator,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _accountActionRequestRepository =
                accountActionRequestRepository;
            _accountTokenGenerator =
                accountTokenGenerator;
            _dbSession = dbSession;
        }

        public async Task<Result<string>> HandleAsync(
            ChangeEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            var user =
                await _userRepository.GetByExternalIdAsync(
                    command.UserExternalId,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure<string>(
                    EmailChangeErrors.UserNotFound);
            }

            var emailResult = User.ValidateAndNormalizeEmail(command.NewEmail);

            if (emailResult.IsFailure)
            {
                return Result.Failure<string>(
                    EmailChangeErrors.InvalidEmail);
            }

            var normalizedEmail = emailResult.Value;

            if (string.Equals(
                user.Email,
                normalizedEmail,
                StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure<string>(
                    EmailChangeErrors.SameEmail);
            }

            var emailExists =
                await _userRepository.EmailExistsAsync(
                    normalizedEmail,
                    user.Id,
                    cancellationToken);

            if (emailExists)
            {
                return Result.Failure<string>(
                    EmailChangeErrors.EmailAlreadyExists);
            }

            var rawToken =
                _accountTokenGenerator.Generate();

            var tokenHash =
                _accountTokenGenerator.Hash(
                    rawToken);

            var metadata =
                JsonSerializer.Serialize(
                    new ChangeEmailMetadata(
                        user.Email,
                        normalizedEmail));

            var createResult =
                AccountActionRequest.Create(
                    user.Id,
                    AccountActionTypes.EmailChange,
                    tokenHash,
                    metadata,
                    DateTime.UtcNow.AddMinutes(30));

            if (createResult.IsFailure)
            {
                return Result.Failure<string>(
                    createResult.Error);
            }

            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                await _accountActionRequestRepository
                    .RevokePendingByUserIdAndTypeAsync(
                        user.Id,
                        AccountActionTypes.EmailChange,
                        cancellationToken);

                await _accountActionRequestRepository
                    .CreateAsync(
                        createResult.Value,
                        cancellationToken);

                await _dbSession.CommitAsync(
                    cancellationToken);

                return Result.Success(
                    rawToken);
            }
            catch
            {
                await _dbSession.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }

        private static string? NormalizeEmail(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalized =
                email.Trim().ToLowerInvariant();

            try
            {
                var mailAddress =
                    new MailAddress(normalized);

                if (!mailAddress.Address.Equals(
                    normalized,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return normalized;
            }
            catch
            {
                return null;
            }
        }
    }
}