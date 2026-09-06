using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Domain.Constants;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;
using System.Text.Json;

namespace Api.Modules.Customer.Application.Auth.ForgotPassword
{
    public sealed class ForgotPasswordHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountActionRequestRepository
            _accountActionRequestRepository;
        private readonly IAccountTokenGenerator
            _accountTokenGenerator;
        private readonly IDbSession _dbSession;

        public ForgotPasswordHandler(
            IUserRepository userRepository,
            IAccountActionRequestRepository
                accountActionRequestRepository,
            IAccountTokenGenerator accountTokenGenerator,
            IDbSession dbSession)
        {
            _userRepository =
                userRepository;

            _accountActionRequestRepository =
                accountActionRequestRepository;

            _accountTokenGenerator =
                accountTokenGenerator;

            _dbSession =
                dbSession;
        }

        public async Task<Result<string?>> HandleAsync(
            ForgotPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var emailResult =
                User.ValidateAndNormalizeEmail(
                    command.Email);

            if (emailResult.IsFailure)
            {
                return Result.Failure<string?>(
                    PasswordResetErrors.InvalidEmail);
            }

            var normalizedEmail =
                emailResult.Value;

            var user =
                await _userRepository.GetByEmailAsync(
                    normalizedEmail,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure<string?>(
                    PasswordResetErrors.EmailNotFound);
            }

            if (user.Status != UserStatus.Active)
            {
                return Result.Success<string?>(
                    null);
            }

            var rawToken =
                _accountTokenGenerator.Generate();

            var tokenHash =
                _accountTokenGenerator.Hash(
                    rawToken);

            var metadata =
                JsonSerializer.Serialize(
                    new PasswordResetMetadata(
                        user.Email));

            var createResult =
                AccountActionRequest.Create(
                    user.Id,
                    AccountActionTypes.PasswordReset,
                    tokenHash,
                    metadata,
                    DateTime.UtcNow.AddMinutes(30));

            if (createResult.IsFailure)
            {
                return Result.Failure<string?>(
                    createResult.Error);
            }

            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                await _accountActionRequestRepository
                    .RevokePendingByUserIdAndTypeAsync(
                        user.Id,
                        AccountActionTypes.PasswordReset,
                        cancellationToken);

                await _accountActionRequestRepository
                    .CreateAsync(
                        createResult.Value,
                        cancellationToken);

                await _dbSession.CommitAsync(
                    cancellationToken);

                /*
                 * Temporary:
                 * returned only so we can test Reset Password
                 * before implementing email delivery.
                 */
                return Result.Success<string?>(
                    rawToken);
            }
            catch
            {
                await _dbSession.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}