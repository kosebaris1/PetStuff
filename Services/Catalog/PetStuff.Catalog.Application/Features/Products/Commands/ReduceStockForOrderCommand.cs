using MediatR;

namespace PetStuff.Catalog.Application.Features.Products.Commands
{
    public class ReduceStockForOrderCommand : IRequest<bool>
    {
        public List<OrderItemStockRequest> Items { get; set; } = new();
    }

    public class OrderItemStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
