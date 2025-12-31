using FluentValidation;
using PetStuff.Catalog.Application.Features.Brands.Commands;

namespace PetStuff.Catalog.Application.Features.Brands.Validators
{
    public class RemoveBrandCommandValidator : AbstractValidator<RemoveBrandCommand>
    {
        public RemoveBrandCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Brand ID must be greater than 0.");
        }
    }
}




