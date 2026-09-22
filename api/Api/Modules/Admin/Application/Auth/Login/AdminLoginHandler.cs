using Api.Modules.Admin.Application.Abstractions.Authentication;
using Api.Modules.Admin.Domain.Entities;
using Api.Modules.Admin.Domain.Enums;
using Api.Modules.Admin.Domain.Repositories;
using Api.Modules.Admin.Infrastructure.Authentication;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Shared.Domain;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Api.Modules.Admin.Application.Auth.Login
{
    public sealed class AdminLoginHandler
    {
        private readonly IAdminUserRepository _userRepository;
        private readonly IAdminRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IAdminJwtService _jwtService;
        private readonly IValidator<AdminLoginCommand> _validator;
        private readonly AdminJwtSettings _jwtSettings;

        public AdminLoginHandler(
            IAdminUserRepository userRepository,
            IAdminRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAdminJwtService jwtService,
            IValidator<AdminLoginCommand> validator,
            IOptions<AdminJwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenGenerator = refreshTokenGenerator;
            _jwtService = jwtService;
            _validator = validator;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<Result<AdminLoginResponse>> HandleAsync(
            AdminLoginCommand command,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(
                command,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                var message = validationResult.Errors.First().ErrorMessage;

                return Result.Failure<AdminLoginResponse>(
                    AdminLoginErrors.Validation(message));
            }

            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user is null)
            {
                return Result.Failure<AdminLoginResponse>(
                    AdminLoginErrors.InvalidCredentials);
            }

            if (user.Status != AdminUserStatus.Active)
            {
                return Result.Failure<AdminLoginResponse>(
                    AdminLoginErrors.InvalidCredentials);
            }

            var passwordValid = _passwordHasher.Verify(
                command.Password,
                user.PasswordHash);

            if (!passwordValid)
            {
                return Result.Failure<AdminLoginResponse>(
                    AdminLoginErrors.InvalidCredentials);
            }

            var refreshTokenExpiresAt = DateTime.UtcNow
                .AddDays(_jwtSettings.RefreshTokenExpirationDays);

            var accessToken = _jwtService.Generate(
                user,
                refreshTokenExpiresAt);

            var generatedRefreshToken = _refreshTokenGenerator.Generate();
            var refreshTokenHash = _refreshTokenGenerator.Hash(generatedRefreshToken.Token);

            var refreshToken = AdminRefreshToken.Create(
                userId: user.Id,
                tokenHash: refreshTokenHash,
                expiresAt: refreshTokenExpiresAt,
                device: command.Device,
                ipAddress: command.IpAddress);

            await _refreshTokenRepository.CreateAsync(
                refreshToken,
                cancellationToken);

            var response = new AdminLoginResponse(
                accessToken.Token,
                accessToken.ExpiresAtUtc,
                generatedRefreshToken.Token,
                refreshTokenExpiresAt);

            return Result.Success(response);
        }
    }
}