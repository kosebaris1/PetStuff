using MediatR;

namespace PetStuff.Basket.Application.Features.Basket.Commands
{
    public class RemoveItemFromBasketCommand : IRequest<bool>
    {
        public string UserId { get; set; } = default!;
        public int ProductId { get; set; }
    }
}

