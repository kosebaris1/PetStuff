using MediatR;
using PetStuff.Basket.Application.Features.Basket.Commands;
using PetStuff.Basket.Application.Interfaces;
using System.Linq;

namespace PetStuff.Basket.Application.Features.Basket.Handlers
{
    public class RemoveItemFromBasketCommandHandler : IRequestHandler<RemoveItemFromBasketCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;

        public RemoveItemFromBasketCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<bool> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetBasketAsync(request.UserId);

            if (basket == null)
            {
                return false;
            }

            var itemToRemove = basket.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (itemToRemove == null)
            {
                return false;
            }

            basket.Items.Remove(itemToRemove);

            await _basketRepository.UpdateBasketAsync(basket);
            return true;
        }
    }
}

