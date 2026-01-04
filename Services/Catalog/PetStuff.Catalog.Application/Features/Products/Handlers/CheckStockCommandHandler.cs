using MediatR;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;

namespace PetStuff.Catalog.Application.Features.Products.Handlers
{
    public class CheckStockCommandHandler : IRequestHandler<CheckStockCommand, bool>
    {
        private readonly IProductRepository _repository;

        public CheckStockCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CheckStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);
            
            if (product == null || !product.IsActive)
            {
                return false;
            }

            return product.Stock >= request.Quantity;
        }
    }
}
