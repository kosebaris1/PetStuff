using MediatR;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;

namespace PetStuff.Catalog.Application.Features.Products.Handlers
{
    public class ReduceStockCommandHandler : IRequestHandler<ReduceStockCommand, bool>
    {
        private readonly IProductRepository _repository;

        public ReduceStockCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ReduceStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);
            
            if (product == null || product.Stock < request.Quantity)
            {
                return false;
            }

            product.Stock -= request.Quantity;
            product.UpdatedDate = DateTime.UtcNow;
            
            await _repository.UpdateProductAsync(product);
            return true;
        }
    }
}
