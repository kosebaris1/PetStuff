using MediatR;

namespace PetStuff.Catalog.Application.Features.Products.Commands
{
    public class CheckStockCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
