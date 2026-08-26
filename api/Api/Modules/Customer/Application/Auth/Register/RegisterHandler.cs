using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Repositories;
using Api.Shared.Database;
using Api.Shared.Domain;
using FluentValidation;

namespace Api.Modules.Customer.Application.Auth.Register
{
    public sealed class RegisterHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDbSession _dbSession;
        private readonly IValidator<RegisterCommand> _validator;

        public RegisterHandler(
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            IPasswordHasher passwordHasher,
            IDbSession dbSession,
            IValidator<RegisterCommand> validator)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _passwordHasher = passwordHasher;
            _dbSession = dbSession;
            _validator = validator;
        }

        public async Task<Result<RegisterResponse>> HandleAsync(
            RegisterCommand command,
            CancellationToken cancellationToken = default)
        {
            var validationResult =
                await _validator.ValidateAsync(
                    command,
                    cancellationToken);

            if (!validationResult.IsValid)
            {
                var message = validationResult
                    .Errors
                    .First()
                    .ErrorMessage;

                return Result.Failure<RegisterResponse>(
                    RegisterErrors.Validation(message));
            }

            var normalizedEmail = command.Email
                .Trim()
                .ToLowerInvariant();

            var emailExists =
                await _userRepository.EmailExistsAsync(
                    normalizedEmail,
                    cancellationToken);

            if (emailExists)
            {
                return Result.Failure<RegisterResponse>(
                    RegisterErrors.EmailAlreadyExists);
            }

            var userResult = User.Create(
                command.Name,
                normalizedEmail);

            if (userResult.IsFailure)
            {
                return Result.Failure<RegisterResponse>(
                    userResult.Error);
            }

            var user = userResult.Value;

            var passwordHash =
                _passwordHasher.Hash(command.Password);

            await _dbSession.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var userId =
                    await _userRepository.CreateAsync(
                        user,
                        cancellationToken);

                var loginResult = Login.Create(
                    userId,
                    passwordHash);

                if (loginResult.IsFailure)
                {
                    await _dbSession.RollbackAsync(
                        cancellationToken);

                    return Result.Failure<RegisterResponse>(
                        loginResult.Error);
                }

                await _loginRepository.CreateAsync(
                    loginResult.Value,
                    cancellationToken);

                await _dbSession.CommitAsync(
                    cancellationToken);

                var response = new RegisterResponse(
                    user.ExternalId,
                    user.Name,
                    user.Email);

                return Result.Success(response);
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