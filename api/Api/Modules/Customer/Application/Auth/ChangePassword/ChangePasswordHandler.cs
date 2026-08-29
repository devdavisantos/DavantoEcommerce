using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Errors;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Domain.Enums;
using Api.Shared.Database;
using Api.Shared.Domain;
using System.IdentityModel.Tokens.Jwt;
using Api.Controllers.Extensions;
using Api.Controllers.Requests.Auth;
using Api.Modules.Customer.Application.Auth.ChangePassword;
using Microsoft.AspNetCore.Authorization;
using LoginEntity = Api.Modules.Customer.Domain.Entities.Login;

namespace Api.Modules.Customer.Application.Auth.ChangePassword
{
    public sealed class ChangePasswordHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly Abstractions.Persistence.IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDbSession _dbSession;

        public ChangePasswordHandler(
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            Abstractions.Persistence.IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _dbSession = dbSession;
        }

        public async Task<Result> HandleAsync(
            ChangePasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var user = await  _userRepository.GetByExternalIdAsync(
                command.UserExternalId,
                cancellationToken);

            if (user is null)
            {
                return Result.Failure(
                    ChangePasswordErrors.UserNotFound);
            }

            if (user.Status != UserStatus.Active)
            {
                return Result.Failure(ChangePasswordErrors.UserInactive);
            }

            var currentLogin = await _loginRepository.GetActiveByUserIdAsync(
                user.Id,
                cancellationToken);

            if (currentLogin is null)
            {
                return Result.Failure(
                    ChangePasswordErrors.LoginNotFound);
            }

            var currentPasswordIsValid = _passwordHasher.Verify(
                command.CurrentPassword,
                currentLogin.PasswordHash);

            if (!currentPasswordIsValid)
            {
                return Result.Failure(
                    ChangePasswordErrors.InvalidCurrentPassword);
            }

            var samePassword = _passwordHasher.Verify(
                command.NewPassword,
                currentLogin.PasswordHash);

            if (samePassword)
            {
                return Result.Failure(
                    ChangePasswordErrors.SamePassword);
            }

            var newPasswordHash = _passwordHasher.Hash(
                command.NewPassword);

            var newLoginResult = LoginEntity.Create(
                user.Id,
                newPasswordHash);

            if (newLoginResult.IsFailure)
            {
                return Result.Failure(
                    newLoginResult.Error);
            }

            await _dbSession.BeginTransactionAsync(cancellationToken);

            try
            {
                await _loginRepository.DeactivateAsync(
                    currentLogin,
                    cancellationToken);

                await _loginRepository.CreateAsync(
                    newLoginResult.Value,
                    cancellationToken);

                await _refreshTokenRepository.RevokeAllByLoginIdAsync(
                    currentLogin.Id,
                    cancellationToken);

                await _dbSession.CommitAsync(cancellationToken);

                return Result.Success();
            }
            catch
            {
                await _dbSession.RollbackAsync(cancellationToken);

                throw;
            }

        }

    }
}
