using FluentValidation;
using PetStuff.Basket.Application.Features.Basket.Commands;

namespace PetStuff.Basket.Application.Features.Basket.Validators
{
    public class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
    {
        public ClearBasketCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}

