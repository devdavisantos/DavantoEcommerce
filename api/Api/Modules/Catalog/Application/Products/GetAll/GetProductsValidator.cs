using FluentValidation;

namespace Api.Modules.Catalog.Application.Products.GetAll
{
    public sealed class GetProductsValidator
        : AbstractValidator<GetProductsQuery>
    {
        public GetProductsValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(255)
                .When(x =>
                    !string.IsNullOrWhiteSpace(x.Search));
        }
    }
}