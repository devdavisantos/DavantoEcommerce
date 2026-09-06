using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Modules.Customer.Domain.Constants;
using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;
using LoginEntity = Api.Modules.Customer.Domain.Entities.Login;

namespace Api.Modules.Customer.Application.Auth.ResetPassword
{
    public sealed class ResetPasswordHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IAccountActionRequestRepository _accountActionRequestRepository;
        private readonly Abstractions.Persistence.IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAccountTokenGenerator _accountTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDbSession _dbSession;

        public ResetPasswordHandler(
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            IAccountActionRequestRepository accountActionRequestRepository,
            Abstractions.Persistence.IRefreshTokenRepository refreshTokenRepository,
            IAccountTokenGenerator accountTokenGenerator,
            IPasswordHasher passwordHasher,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _accountActionRequestRepository =
                accountActionRequestRepository;
            _refreshTokenRepository =
                refreshTokenRepository;
            _accountTokenGenerator =
                accountTokenGenerator;
            _passwordHasher =
                passwordHasher;
            _dbSession =
                dbSession;
        }

        public async Task<Result> HandleAsync(
            ResetPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
            {
                return Result.Failure(
                    PasswordResetErrors.InvalidToken);
            }

            var tokenHash =
                _accountTokenGenerator.Hash(
                    command.Token);

            var actionRequest =
                await _accountActionRequestRepository
                    .GetByTokenHashAsync(
                        tokenHash,
                        AccountActionTypes.PasswordReset,
                        cancellationToken);

            if (actionRequest is null)
            {
                return Result.Failure(
                    PasswordResetErrors.InvalidToken);
            }

            if (actionRequest.IsCompleted())
            {
                return Result.Failure(
                    PasswordResetErrors.AlreadyCompleted);
            }

            if (actionRequest.IsRevoked())
            {
                return Result.Failure(
                    PasswordResetErrors.RevokedToken);
            }

            if (actionRequest.IsExpired())
            {
                return Result.Failure(
                    PasswordResetErrors.ExpiredToken);
            }

            var user =
                await _userRepository.GetByIdAsync(
                    actionRequest.UserId,
                    cancellationToken);

            if (user is null)
            {
                return Result.Failure(
                    PasswordResetErrors.UserNotFound);
            }

            if (user.Status != UserStatus.Active)
            {
                return Result.Failure(
                    PasswordResetErrors.UserInactive);
            }

            var currentLogin =
                await _loginRepository
                    .GetActiveByUserIdAsync(
                        user.Id,
                        cancellationToken);

            if (currentLogin is null)
            {
                return Result.Failure(
                    PasswordResetErrors.LoginNotFound);
            }

            var samePassword =
                _passwordHasher.Verify(
                    command.NewPassword,
                    currentLogin.PasswordHash);

            if (samePassword)
            {
                return Result.Failure(
                    PasswordResetErrors.SamePassword);
            }

            var newPasswordHash =
                _passwordHasher.Hash(
                    command.NewPassword);

            var newLoginResult =
                LoginEntity.Create(
                    user.Id,
                    newPasswordHash);

            if (newLoginResult.IsFailure)
            {
                return Result.Failure(
                    newLoginResult.Error);
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
                await _loginRepository.DeactivateAsync(
                    currentLogin,
                    cancellationToken);

                await _loginRepository.CreateAsync(
                    newLoginResult.Value,
                    cancellationToken);

                await _refreshTokenRepository
                    .RevokeAllByLoginIdAsync(
                        currentLogin.Id,
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