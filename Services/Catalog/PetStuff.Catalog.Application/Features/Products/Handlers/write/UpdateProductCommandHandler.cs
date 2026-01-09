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
            // ProductRepository zaten mevcut image'ları silecek, bu yüzden sadece yeni image'ları set et
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                product.Images = new List<ProductImage>();
                
                // Yeni image'ları ekle
                for (int i = 0; i < request.ImageUrls.Count; i++)
                {
                    var imageUrl = request.ImageUrls[i]?.Trim();
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        product.Images.Add(new ProductImage
                        {
                            ImageUrl = imageUrl,
                            IsMain = i == 0, // İlk resmi main yap
                            ProductId = product.Id,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
            }
            else
            {
                // ImageUrls null veya boşsa, mevcut image'ları koru (null set et, ProductRepository mevcut image'ları koruyacak)
                product.Images = null;
            }

            await _repository.UpdateProductAsync(product);

            return true;
        }
    }
}
