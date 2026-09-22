using Api.Modules.Admin.Domain.Repositories;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Shared.Domain;

namespace Api.Modules.Admin.Application.Auth.Logout
{
    public sealed class AdminLogoutHandler
    {
        private readonly IAdminRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public AdminLogoutHandler(
            IAdminRefreshTokenRepository refreshTokenRepository,
            IRefreshTokenGenerator refreshTokenGenerator)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<Result> HandleAsync(
            AdminLogoutCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.RefreshToken))
            {
                return Result.Failure(
                    AdminLogoutErrors.InvalidToken);
            }

            var tokenHash = _refreshTokenGenerator.Hash(command.RefreshToken);

            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (refreshToken is null)
            {
                return Result.Failure(
                    AdminLogoutErrors.InvalidToken);
            }

            if (refreshToken.IsExpired() ||
                refreshToken.IsRevoked() ||
                refreshToken.IsReplaced())
            {
                return Result.Failure(
                    AdminLogoutErrors.InvalidToken);
            }

            await _refreshTokenRepository.RevokeAsync(
                refreshToken.Id,
                cancellationToken);

            return Result.Success();
        }
    }
}