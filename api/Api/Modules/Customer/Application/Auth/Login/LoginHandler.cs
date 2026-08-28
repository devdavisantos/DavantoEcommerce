using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Domain.Enums;
using Api.Shared.Domain;
using FluentValidation;

namespace Api.Modules.Customer.Application.Auth.Login
{
    public sealed class LoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IValidator<LoginCommand> _Validator;

        public LoginHandler(
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IValidator<LoginCommand> validator)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _Validator = validator;
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

            if (user.Status != UserStatus.Active)
            {
                return Result.Failure<LoginResponse>(LoginErrors.AccountInactive);
            }

            if (user.Status == UserStatus.Blocked)
            {
                return Result.Failure<LoginResponse>(LoginErrors.AccountBlocked);
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

            var accessToken = _jwtService.GenerateAccessToken(user);

            var response = new LoginResponse(accessToken.Token, accessToken.ExpiresAtUtc);

            return Result.Success(response);
        }

    }
}
