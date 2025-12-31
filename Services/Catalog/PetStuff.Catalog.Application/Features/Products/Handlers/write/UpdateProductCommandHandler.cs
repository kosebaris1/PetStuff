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

            // Update images if provided - Önce mevcut image'ları temizle, sonra yenilerini ekle
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                // Mevcut image'ları temizle
                if (product.Images != null && product.Images.Any())
                {
                    product.Images.Clear();
                }
                else
                {
                    product.Images = new List<ProductImage>();
                }

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
                            ProductId = product.Id
                        });
                    }
                }
            }
            else
            {
                // ImageUrls boşsa veya null ise, tüm image'ları temizle
                if (product.Images != null)
                {
                    product.Images.Clear();
                }
            }

            await _repository.UpdateProductAsync(product);

            return true;
        }
    }
}
