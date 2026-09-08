using Api.Modules.Customer.Application.Common.Validation;
using FluentValidation;

namespace Api.Modules.Customer.Application.Auth.ChangePassword
{
    public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(
                    "Current password is required.");

            RuleFor(x => x.NewPassword)
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
