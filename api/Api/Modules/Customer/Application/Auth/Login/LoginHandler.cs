using Api.Modules.Customer.Application.Abstractions.Persistence;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Enums;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Domain;
using FluentValidation;
using Api.Configuration;

namespace Api.Modules.Customer.Application.Auth.Login
{
    public sealed class LoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IValidator<LoginCommand> _Validator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly Abstractions.Persistence.IRefreshTokenRepository _refreshTokenRepository;

        public LoginHandler(
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IValidator<LoginCommand> validator,
            IRefreshTokenGenerator refreshTokenGenerator,
            Abstractions.Persistence.IRefreshTokenRepository refreshTokenRepository
            )
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _Validator = validator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _Validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var message = validationResult.Errors.First().ErrorMessage;

                return Result.Failure<LoginResponse>(LoginErrors.Validation(message));
            }

            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null)
            {
                return Result.Failure<LoginResponse>(LoginErrors.InvalidCredentials);
            }

            if (user.Status == UserStatus.Blocked)
            {
                return Result.Failure<LoginResponse>(LoginErrors.AccountBlocked);
            }

            if (user.Status != UserStatus.Active)
            {
                return Result.Failure<LoginResponse>(LoginErrors.AccountInactive);
            }

            var login = await _loginRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

            if (login is null)
            {
                return Result.Failure<LoginResponse>(LoginErrors.InvalidCredentials);
            }

            var passwordValid = _passwordHasher.Verify(command.Password, login.PasswordHash);

            if (!passwordValid)
            {
                return Result.Failure<LoginResponse>(LoginErrors.InvalidCredentials);
            }

            login.RegisterLogin();

            await _loginRepository.UpdateLastLoginAsync(login, cancellationToken);



            var generatedRefreshToken = _refreshTokenGenerator.Generate();
            var accessToken = _jwtService.GenerateAccessToken(user, generatedRefreshToken.ExpiresAtUtc);

            var refreshTokenHash = _refreshTokenGenerator.Hash(generatedRefreshToken.Token);

            var refreshTokenResult = RefreshToken.Create(
                loginId: login.Id,
                tokenHash: refreshTokenHash,
                expiresAt: generatedRefreshToken.ExpiresAtUtc,
                device: command.Device,
                ipAddress: command.IpAddress);

            if (refreshTokenResult.IsFailure)
            {
                return Result.Failure<LoginResponse>(
                    refreshTokenResult.Error);
            }

            await _refreshTokenRepository.CreateAsync(refreshTokenResult.Value, cancellationToken);

            var response = new LoginResponse(
                accessToken.Token,
                accessToken.ExpiresAtUtc,
                generatedRefreshToken.Token,
                generatedRefreshToken.ExpiresAtUtc);

            return Result.Success(response);
        }

    }
}


