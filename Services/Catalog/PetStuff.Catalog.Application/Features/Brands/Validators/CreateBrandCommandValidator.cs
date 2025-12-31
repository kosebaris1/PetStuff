using FluentValidation;
using PetStuff.Catalog.Application.Features.Brands.Commands;

namespace PetStuff.Catalog.Application.Features.Brands.Validators
{
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required.")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters.");
        }
    }
}




