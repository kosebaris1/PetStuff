using MediatR;
using PetStuff.Basket.Application.Features.Basket.Commands;
using PetStuff.Basket.Application.Interfaces;
using System.Linq;
using BasketEntity = PetStuff.Basket.Domain.Entities.Basket;
using BasketItem = PetStuff.Basket.Domain.Entities.BasketItem;

namespace PetStuff.Basket.Application.Features.Basket.Handlers
{
    public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;

        public AddItemToBasketCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<bool> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                throw new ArgumentException("User ID is required.", nameof(request.UserId));
            }

            var basket = await _basketRepository.GetBasketAsync(request.UserId!);

            if (basket == null)
            {
                basket = new BasketEntity
                {
                    UserId = request.UserId!,
                    Items = new List<BasketItem>()
                };
            }

            var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                basket.Items.Add(new BasketItem
                {
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    Price = request.Price,
                    Quantity = request.Quantity,
                    ProductImageUrl = request.ProductImageUrl
                });
            }

            await _basketRepository.UpdateBasketAsync(basket);
            return true;
        }
    }
}

