using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Email.Change;
using Api.Modules.Customer.Domain.Constants;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;
using System.Text.Json;

namespace Api.Modules.Customer.Application.Email.Confirm
{
    public sealed class ConfirmEmailChangeHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountActionRequestRepository
            _accountActionRequestRepository;
        private readonly IAccountTokenGenerator
            _accountTokenGenerator;
        private readonly IDbSession _dbSession;

        public ConfirmEmailChangeHandler(
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

        public async Task<Result> HandleAsync(
            ConfirmEmailChangeCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
            {
                return Result.Failure(
                    EmailChangeErrors.InvalidToken);
            }

            var tokenHash =
                _accountTokenGenerator.Hash(
                    command.Token);

            var actionRequest =
                await _accountActionRequestRepository
                    .GetByTokenHashAsync(
                        tokenHash,
                        AccountActionTypes.EmailChange,
                        cancellationToken);

            if (actionRequest is null)
            {
                return Result.Failure(
                    EmailChangeErrors.InvalidToken);
            }

            if (actionRequest.IsCompleted())
            {
                return Result.Failure(
                    EmailChangeErrors.AlreadyConfirmed);
            }

            if (actionRequest.IsRevoked())
            {
                return Result.Failure(
                    EmailChangeErrors.RevokedToken);
            }

            if (actionRequest.IsExpired())
            {
                return Result.Failure(
                    EmailChangeErrors.ExpiredToken);
            }

            if (string.IsNullOrWhiteSpace(
                actionRequest.Metadata))
            {
                return Result.Failure(
                    EmailChangeErrors.InvalidMetadata);
            }

            ChangeEmailMetadata? metadata;

            try
            {
                metadata =
                    JsonSerializer.Deserialize<
                        ChangeEmailMetadata>(
                        actionRequest.Metadata);
            }
            catch (JsonException)
            {
                return Result.Failure(
                    EmailChangeErrors.InvalidMetadata);
            }

            if (metadata is null ||
                string.IsNullOrWhiteSpace(
                    metadata.NewEmail))
            {
                return Result.Failure(
                    EmailChangeErrors.InvalidMetadata);
            }

            var user =
                await _userRepository.GetByIdAsync(
                    actionRequest.UserId,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure(
                    EmailChangeErrors.UserNotFound);
            }

            var emailResult =
                User.ValidateAndNormalizeEmail(
                    metadata.NewEmail);

            if (emailResult.IsFailure)
            {
                return Result.Failure(
                    EmailChangeErrors.InvalidEmail);
            }

            var normalizedEmail =
                emailResult.Value;

            var emailExists =
                await _userRepository.EmailExistsAsync(
                    normalizedEmail,
                    cancellationToken);

            if (emailExists)
            {
                return Result.Failure(
                    EmailChangeErrors.EmailAlreadyExists);
            }

            var changeEmailResult =
                user.ChangeEmail(
                    normalizedEmail);

            if (changeEmailResult.IsFailure)
            {
                return Result.Failure(
                    changeEmailResult.Error);
            }

            var completeResult =
                actionRequest.Complete();

            if (completeResult.IsFailure)
            {
                return Result.Failure(
                    completeResult.Error);
            }

            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                await _userRepository.UpdateAsync(
                    user,
                    cancellationToken);

                await _accountActionRequestRepository
                    .UpdateAsync(
                        actionRequest,
                        cancellationToken);

                await _dbSession.CommitAsync(
                    cancellationToken);

                return Result.Success();
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