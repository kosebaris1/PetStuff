using MediatR;
using PetStuff.Order.Application.DTOs;

namespace PetStuff.Order.Application.Features.Orders.Queries
{
    public class GetAllOrdersQuery : IRequest<List<OrderDto>>
    {
    }
}
