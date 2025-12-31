using FluentValidation;
using PetStuff.Catalog.Application.Features.Categories.Commands;

namespace PetStuff.Catalog.Application.Features.Categories.Validators
{
    public class RemoveCategoryCommandValidator : AbstractValidator<RemoveCategoryCommand>
    {
        public RemoveCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
        }
    }
}




