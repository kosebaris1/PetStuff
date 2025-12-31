using AutoMapper;
using MediatR;
using PetStuff.Order.Application.DTOs;
using PetStuff.Order.Application.Features.Orders.Queries;
using PetStuff.Order.Application.Interfaces;

namespace PetStuff.Order.Application.Features.Orders.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order == null || order.UserId != request.UserId)
            {
                return null;
            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}



