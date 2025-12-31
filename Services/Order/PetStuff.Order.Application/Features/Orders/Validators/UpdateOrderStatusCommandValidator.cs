using FluentValidation;
using PetStuff.Order.Application.Features.Orders.Commands;

namespace PetStuff.Order.Application.Features.Orders.Validators
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order ID must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid order status.");
        }
    }
}



