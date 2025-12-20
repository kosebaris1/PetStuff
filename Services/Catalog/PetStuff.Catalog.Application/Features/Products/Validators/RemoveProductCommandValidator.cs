using FluentValidation;
using PetStuff.Catalog.Application.Features.Products.Commands;

namespace PetStuff.Catalog.Application.Features.Products.Validators
{
    public class RemoveProductCommandValidator : AbstractValidator<RemoveProductCommand>
    {
        public RemoveProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");
        }
    }
}
