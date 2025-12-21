using MediatR;
using OrderStatus = PetStuff.Order.Domain.Entities.OrderStatus;

namespace PetStuff.Order.Application.Features.Orders.Commands
{
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}

