using FluentValidation;
using PetStuff.Basket.Application.Features.Basket.Queries;

namespace PetStuff.Basket.Application.Features.Basket.Validators
{
    public class GetBasketQueryValidator : AbstractValidator<GetBasketQuery>
    {
        public GetBasketQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}

