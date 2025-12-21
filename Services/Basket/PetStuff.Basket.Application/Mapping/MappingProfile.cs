using AutoMapper;
using PetStuff.Basket.Application.DTOs;
using BasketEntity = PetStuff.Basket.Domain.Entities.Basket;
using BasketItemEntity = PetStuff.Basket.Domain.Entities.BasketItem;

namespace PetStuff.Basket.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BasketEntity, BasketDto>();
            CreateMap<BasketItemEntity, BasketItemDto>();
        }
    }
}

