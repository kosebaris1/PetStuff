using MediatR;
using PetStuff.Basket.Application.Features.Basket.Commands;
using PetStuff.Basket.Application.Interfaces;

namespace PetStuff.Basket.Application.Features.Basket.Handlers
{
    public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;

        public UpdateItemQuantityCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<bool> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetBasketAsync(request.UserId);

            if (basket == null)
            {
                return false;
            }

            var item = basket.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (item == null)
            {
                return false;
            }

            if (request.Quantity <= 0)
            {
                basket.Items.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }

            await _basketRepository.UpdateBasketAsync(basket);
            return true;
        }
    }
}

