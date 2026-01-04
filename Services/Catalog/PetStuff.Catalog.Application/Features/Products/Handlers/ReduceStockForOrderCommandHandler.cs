using MediatR;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;

namespace PetStuff.Catalog.Application.Features.Products.Handlers
{
    public class ReduceStockForOrderCommandHandler : IRequestHandler<ReduceStockForOrderCommand, bool>
    {
        private readonly IProductRepository _repository;

        public ReduceStockForOrderCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ReduceStockForOrderCommand request, CancellationToken cancellationToken)
        {
            // Önce tüm ürünlerin stok kontrolünü yap
            foreach (var item in request.Items)
            {
                var product = await _repository.GetByIdAsync(item.ProductId);
                if (product == null || !product.IsActive || product.Stock < item.Quantity)
                {
                    return false;
                }
            }

            // Tüm stoklar yeterliyse, hepsini düşür
            foreach (var item in request.Items)
            {
                var product = await _repository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    product.UpdatedDate = DateTime.UtcNow;
                    await _repository.UpdateProductAsync(product);
                }
            }

            return true;
        }
    }
}
