using AutoMapper;
using PetStuff.Order.Application.DTOs;
using OrderEntity = PetStuff.Order.Domain.Entities.Order;
using OrderItemEntity = PetStuff.Order.Domain.Entities.OrderItem;

namespace PetStuff.Order.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderEntity, OrderDto>();
            CreateMap<OrderItemEntity, OrderItemDto>();
        }
    }
}
