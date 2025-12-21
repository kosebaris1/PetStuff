using MediatR;
using PetStuff.Basket.Application.DTOs;

namespace PetStuff.Basket.Application.Features.Basket.Queries
{
    public class GetBasketQuery : IRequest<BasketDto?>
    {
        public string UserId { get; set; } = default!;
    }
}

