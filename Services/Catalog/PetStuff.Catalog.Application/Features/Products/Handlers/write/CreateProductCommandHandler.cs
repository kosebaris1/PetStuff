using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;
using PetStuff.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Application.Features.Products.Handlers.write
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);

            // ImageUrls varsa ProductImage listesi oluştur
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                product.Images = new List<ProductImage>();
                
                for (int i = 0; i < request.ImageUrls.Count; i++)
                {
                    var imageUrl = request.ImageUrls[i]?.Trim();
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        product.Images.Add(new ProductImage
                        {
                            ImageUrl = imageUrl,
                            IsMain = i == 0 // İlk resmi main yap
                            // ProductId otomatik set edilecek (EF Core cascade save)
                        });
                    }
                }
            }

            await _repository.CreateProductAsync(product);

            return Unit.Value;

        }
    }
}
