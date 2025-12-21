using MediatR;

namespace PetStuff.Basket.Application.Features.Basket.Commands
{
    public class UpdateItemQuantityCommand : IRequest<bool>
    {
        public string UserId { get; set; } = default!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

