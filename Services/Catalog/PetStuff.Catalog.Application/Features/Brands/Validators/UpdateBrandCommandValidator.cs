using FluentValidation;
using PetStuff.Catalog.Application.Features.Brands.Commands;

namespace PetStuff.Catalog.Application.Features.Brands.Validators
{
    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Brand ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required.")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters.");
        }
    }
}




