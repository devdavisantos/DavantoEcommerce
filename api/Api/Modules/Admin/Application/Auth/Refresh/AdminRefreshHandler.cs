using Api.Modules.Admin.Application.Abstractions.Authentication;
using Api.Modules.Admin.Domain.Entities;
using Api.Modules.Admin.Domain.Enums;
using Api.Modules.Admin.Domain.Repositories;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Shared.Database;
using Api.Shared.Domain;

namespace Api.Modules.Admin.Application.Auth.Refresh
{
    public sealed class AdminRefreshHandler
    {
        private readonly IAdminUserRepository _userRepository;
        private readonly IAdminRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IAdminJwtService _jwtService;
        private readonly IDbSession _dbSession;

        public AdminRefreshHandler(
            IAdminUserRepository userRepository,
            IAdminRefreshTokenRepository refreshTokenRepository,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAdminJwtService jwtService,
            IDbSession dbSession)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
            _jwtService = jwtService;
            _dbSession = dbSession;
        }

        public async Task<Result<AdminRefreshResponse>> HandleAsync(
            AdminRefreshCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.RefreshToken))
            {
                return Result.Failure<AdminRefreshResponse>(
                    AdminRefreshErrors.InvalidToken);
            }

            var tokenHash = _refreshTokenGenerator.Hash(command.RefreshToken);

            var currentRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (currentRefreshToken is null)
            {
                return Result.Failure<AdminRefreshResponse>(
                    AdminRefreshErrors.InvalidToken);
            }

            if (currentRefreshToken.IsExpired())
            {
                return Result.Failure<AdminRefreshResponse>(
                    AdminRefreshErrors.ExpiredToken);
            }

            if (currentRefreshToken.IsRevoked())
            {
                return Result.Failure<AdminRefreshResponse>(
                    AdminRefreshErrors.RevokedToken);
            }

            if (currentRefreshToken.IsReplaced())
            {
                return Result.Failure<AdminRefreshResponse>(
                    AdminRefreshErrors.ReplacedToken);
            }

            var user = await _userRepository.GetByIdAsync(
                currentRefreshToken.UserId);

            if (user is null || user.Status != AdminUserStatus.Active)
            {
                return Result.Failure<AdminRefreshResponse>(
                    AdminRefreshErrors.InvalidSession);
            }

            var generatedRefreshToken = _refreshTokenGenerator.Generate();
            var newRefreshTokenHash = _refreshTokenGenerator.Hash(generatedRefreshToken.Token);

            var newRefreshToken = AdminRefreshToken.Create(
                userId: user.Id,
                tokenHash: newRefreshTokenHash,
                expiresAt: currentRefreshToken.ExpiresAt,
                device: currentRefreshToken.Device,
                ipAddress: currentRefreshToken.IpAddress);

            var accessToken = _jwtService.Generate(
                user,
                currentRefreshToken.ExpiresAt);

            try
            {
                await _dbSession.OpenConnectionAsync(cancellationToken);
                await _dbSession.BeginTransactionAsync(cancellationToken);



                var replaced = await _refreshTokenRepository.ReplaceAsync(
                    currentRefreshToken,
                    newRefreshToken,
                    cancellationToken);

                if (!replaced)
                {
                    await _dbSession.RollbackAsync(CancellationToken.None);
                    return Result.Failure<AdminRefreshResponse>(AdminRefreshErrors.ReplacedToken);
                }
                await _dbSession.CommitAsync(cancellationToken);
            }
            catch
            {
                await _dbSession.RollbackAsync(CancellationToken.None);
                throw;
            }

            var response = new AdminRefreshResponse(
                accessToken.Token,
                accessToken.ExpiresAtUtc,
                generatedRefreshToken.Token,
                currentRefreshToken.ExpiresAt);

            return Result.Success(response);
        }
    }
}

