using Api.Modules.Customer.Application.Common.Validation;
using FluentValidation;

namespace Api.Modules.Customer.Application.Auth.Register
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(200)
                .WithMessage("Name cannot exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email is invalid.")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(
                    PasswordPolicy.RequiredMessage)
                .MinimumLength(
                    PasswordPolicy.MinimumLength)
                .WithMessage(
                    PasswordPolicy.MinimumLengthMessage)
                .MaximumLength(
                    PasswordPolicy.MaximumLength)
                .WithMessage(
                    PasswordPolicy.MaximumLengthMessage);
        }
    }
}
