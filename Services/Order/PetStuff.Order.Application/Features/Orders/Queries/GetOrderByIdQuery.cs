using MediatR;
using PetStuff.Order.Application.DTOs;

namespace PetStuff.Order.Application.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto?>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = default!;
    }
}



