using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.DTOs;
using PetStuff.Catalog.Application.Features.Products.Queries;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;

namespace PetStuff.Catalog.Application.Features.Products.Handlers.read
{
    public class GetProductsByBrandQueryHandler : IRequestHandler<GetProductsByBrandQuery, List<ProductListDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductsByBrandQueryHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ProductListDto>> Handle(GetProductsByBrandQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllAsync();
            var filteredProducts = products.Where(p => p.BrandId == request.BrandId).ToList();
            return _mapper.Map<List<ProductListDto>>(filteredProducts);
        }
    }
}




