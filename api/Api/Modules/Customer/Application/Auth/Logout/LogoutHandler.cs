using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Modules.Customer.Domain.Errors;
using Api.Shared.Domain;

namespace Api.Modules.Customer.Application.Auth.Logout
{
    public sealed class LogoutHandler
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public LogoutHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IRefreshTokenGenerator refreshTokenGenerator)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<Result> HandleAsync(
            LogoutCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.RefreshToken))
            {
                return Result.Failure(RefreshTokenErrors.InvalidToken);
            }

            var tokenHash = _refreshTokenGenerator.Hash(command.RefreshToken);

            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(tokenHash, cancellationToken);

            if (refreshToken is null)
            {
                return Result.Failure(RefreshTokenErrors.InvalidToken);
            }

            await _refreshTokenRepository.RevokeAsync(refreshToken.Id, cancellationToken);

            return Result.Success();
        }

        








    }
}
