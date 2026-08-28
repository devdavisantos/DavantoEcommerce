using FluentValidation;

namespace Api.Modules.Customer.Application.Auth.Login
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Emails is required.")
                .EmailAddress()
                .WithMessage("Email is invalid.")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MaximumLength(128)
                .WithMessage("Password cannot exceed 128 characters.");
        }
    }
}
