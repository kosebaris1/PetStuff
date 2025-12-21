using MediatR;
using PetStuff.Basket.Application.Features.Basket.Commands;
using PetStuff.Basket.Application.Interfaces;

namespace PetStuff.Basket.Application.Features.Basket.Handlers
{
    public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;

        public ClearBasketCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<bool> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
        {
            return await _basketRepository.DeleteBasketAsync(request.UserId);
        }
    }
}

