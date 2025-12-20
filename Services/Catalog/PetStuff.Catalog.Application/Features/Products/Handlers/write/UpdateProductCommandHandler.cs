using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;
using PetStuff.Catalog.Domain.Entities;

namespace PetStuff.Catalog.Application.Features.Products.Handlers.write
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand,bool>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public UpdateProductCommandHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);
            if (product == null) return false;

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.IsActive = request.IsActive;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.UpdatedDate = DateTime.UtcNow;

            // Update images if provided
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                product.Images = request.ImageUrls
                    .Select(url => new ProductImage
                    {
                        ImageUrl = url,
                        IsMain = false,
                        ProductId = product.Id
                    }).ToList();
            }

            await _repository.UpdateProductAsync(product);

            return true;
        }
    }
}
