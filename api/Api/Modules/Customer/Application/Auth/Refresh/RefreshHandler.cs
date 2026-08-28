using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Authentication;
using Api.Modules.Customer.Infrastructure.Persistence;
using Api.Shared.Domain;
using Api.Modules.Customer.Domain.Errors;
using System.Net;

namespace Api.Modules.Customer.Application.Auth.Refresh
{
    public sealed class RefreshHandler
    {
        private readonly Abstractions.Persistence.IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ILoginRepository _loginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshHandler(
            Abstractions.Persistence.IRefreshTokenRepository refreshTokenRepository,
            IRefreshTokenGenerator refreshTokenGenerator,
            ILoginRepository loginRepository,
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
            _loginRepository = loginRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<RefreshResponse>> HandleAsync(
            RefreshCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.RefreshToken))
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.InvalidToken);
            }

            var tokenHash = _refreshTokenGenerator.Hash(command.RefreshToken);

            var currentRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(tokenHash, cancellationToken);

            if (currentRefreshToken is null)
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.InvalidToken);
            }

            if (currentRefreshToken.IsExpired())
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.ExpiredToken);
            }

            if (currentRefreshToken.IsRevoked())
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.RevokedToken);
            }

            if (currentRefreshToken.IsReplaced())
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.ReplacedToken);
            }

            var login = await _loginRepository.GetByIdAsync(currentRefreshToken.LoginId, cancellationToken);

            if (login is null)
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.InvalidToken);
            }

            var user = await _userRepository.GetByIdAsync(login.UserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<RefreshResponse>(RefreshTokenErrors.InvalidToken);
            }

            var accessToken = _jwtService.GenerateAccessToken(user);

            var generatedRefreshToken = _refreshTokenGenerator.Generate();

            var newRefreshTokenHash = _refreshTokenGenerator.Hash(generatedRefreshToken.Token);

            var newRefreshTokenResult = RefreshToken.Create(
                loginId: login.Id,
                tokenHash: newRefreshTokenHash,
                expiresAt: generatedRefreshToken.ExpiresAtUtc,
                device: currentRefreshToken.Device,
                ipAddress: currentRefreshToken.IpAddress
                );

            if (newRefreshTokenResult.IsFailure)
            {
                return Result.Failure<RefreshResponse>(newRefreshTokenResult.Error);
            }

            await _refreshTokenRepository.CreateAsync(newRefreshTokenResult.Value, cancellationToken);

            await _refreshTokenRepository.ReplaceAsync(currentRefreshToken.Id, DateTime.UtcNow, cancellationToken);

            return Result.Success(
                new RefreshResponse(
                    accessToken.Token,
                    accessToken.ExpiresAtUtc,
                    generatedRefreshToken.Token));

        }

    }
}
