using Api.Modules.Customer.Application.Common.Validation;
using FluentValidation;

namespace Api.Modules.Customer.Application.Auth.ResetPassword
{
    public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Password reset token is required.");

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
