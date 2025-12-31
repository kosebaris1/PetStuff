using MediatR;

namespace PetStuff.Basket.Application.Features.Basket.Commands
{
    public class AddItemToBasketCommand : IRequest<bool>
    {
        public string? UserId { get; set; } // Controller'da set edilecek, nullable
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}

