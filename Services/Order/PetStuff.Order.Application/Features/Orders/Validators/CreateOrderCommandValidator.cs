using FluentValidation;
using PetStuff.Order.Application.Features.Orders.Commands;

namespace PetStuff.Order.Application.Features.Orders.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            // UserId ve Items token'dan ve Basket Service'den otomatik alınacak

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MaximumLength(500).WithMessage("Shipping address cannot exceed 500 characters.");
        }
    }
}

