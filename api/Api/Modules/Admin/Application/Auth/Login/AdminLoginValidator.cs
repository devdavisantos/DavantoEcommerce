using FluentValidation;

namespace Api.Modules.Admin.Application.Auth.Login
{
    public sealed class AdminLoginValidator : AbstractValidator<AdminLoginCommand>
    {
        public AdminLoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}