using FluentValidation;
using PetStuff.Basket.Application.Features.Basket.Commands;

namespace PetStuff.Basket.Application.Features.Basket.Validators
{
    public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
    {
        public RemoveItemFromBasketCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");
        }
    }
}

