using MediatR;
using PetStuff.Order.Application.DTOs;
using System.Collections.Generic;

namespace PetStuff.Order.Application.Features.Orders.Queries
{
    public class GetOrdersByUserIdQuery : IRequest<List<OrderDto>>
    {
        public string UserId { get; set; } = default!;
    }
}



