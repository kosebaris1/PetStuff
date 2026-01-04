using AutoMapper;
using MediatR;
using PetStuff.Order.Application.DTOs;
using PetStuff.Order.Application.Features.Orders.Queries;
using PetStuff.Order.Application.Interfaces;
using System.Linq;

namespace PetStuff.Order.Application.Features.Orders.Handlers
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            // Bu metod repository'de yok, eklememiz gerekiyor
            // Şimdilik GetAllAsync ekleyelim
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}
