using AutoMapper;
using MediatR;
using PetStuff.Basket.Application.DTOs;
using PetStuff.Basket.Application.Features.Basket.Queries;
using PetStuff.Basket.Application.Interfaces;

namespace PetStuff.Basket.Application.Features.Basket.Handlers
{
    public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, BasketDto?>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public GetBasketQueryHandler(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<BasketDto?> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetBasketAsync(request.UserId);

            if (basket == null)
            {
                return null;
            }

            var basketDto = _mapper.Map<BasketDto>(basket);
            return basketDto;
        }
    }
}

