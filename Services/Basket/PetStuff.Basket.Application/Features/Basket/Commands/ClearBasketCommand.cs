using MediatR;

namespace PetStuff.Basket.Application.Features.Basket.Commands
{
    public class ClearBasketCommand : IRequest<bool>
    {
        public string UserId { get; set; } = default!;
    }
}

