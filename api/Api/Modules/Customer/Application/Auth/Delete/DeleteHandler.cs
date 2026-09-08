using Api.Modules.Customer.Application.Profile;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Shared.Database;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Auth.Delete
{
    public sealed class DeleteHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly Abstractions.Persistence.IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDbSession _dbSession;

        public DeleteHandler(
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            Abstractions.Persistence.IRefreshTokenRepository refreshTokenRepository,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _dbSession = dbSession;
        }

        public async Task<Result> HandleAsync(
            DeleteCommand command,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByExternalIdAsync(
                command.UserExternalId,
                cancellationToken);

            if (user is null)
            {
                return Result.Failure(
                    ProfileErrors.UserNotFound);
            }

            var login = await _loginRepository.GetActiveByUserIdAsync(
                user.Id,
                cancellationToken);

            await _dbSession.BeginTransactionAsync(cancellationToken);

            try
            {
                var deleteResult = user.Delete();

                if (deleteResult.IsFailure)
                {
                    await _dbSession.RollbackAsync(cancellationToken);

                    return deleteResult;
                }

                await _userRepository.UpdateAsync(user, cancellationToken);

                if (login is not null)
                {
                    var deactivateResult = login.Deactivate();

                    if (deactivateResult.IsFailure)
                    {
                        await _dbSession.RollbackAsync(cancellationToken);

                        return Result.Failure(deactivateResult.Error);
                    }

                    await _loginRepository.DeactivateAsync(login, cancellationToken);

                    await _refreshTokenRepository.RevokeAllByLoginIdAsync(login.Id, cancellationToken);
                }

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
