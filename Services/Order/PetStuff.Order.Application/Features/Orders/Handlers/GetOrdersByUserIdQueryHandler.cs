using AutoMapper;
using MediatR;
using PetStuff.Order.Application.DTOs;
using PetStuff.Order.Application.Features.Orders.Queries;
using PetStuff.Order.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PetStuff.Order.Application.Features.Orders.Handlers
{
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersByUserIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByUserIdAsync(request.UserId);
            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}



