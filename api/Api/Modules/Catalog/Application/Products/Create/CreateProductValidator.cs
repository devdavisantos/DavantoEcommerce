using FluentValidation;

namespace Api.Modules.Catalog.Application.Products.Create
{
    public sealed class CreateProductValidator
        : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Description)
                .NotNull();

            RuleFor(x => x.Sku)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Sku));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DiscountPrice)
                .GreaterThanOrEqualTo(0)
                .LessThan(x => x.Price)
                .When(x => x.DiscountPrice.HasValue);

            RuleFor(x => x.Condition)
                .IsInEnum();

            RuleFor(x => x.Images)
                .NotEmpty()
                .WithMessage("At least one product image is required.")
                .Must(images => images.Count <= 10)
                .WithMessage("A product can have a maximum of 10 images.");
        }
    }
}