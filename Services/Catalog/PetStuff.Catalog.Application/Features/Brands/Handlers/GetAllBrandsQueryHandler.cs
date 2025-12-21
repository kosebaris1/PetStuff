using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.DTOs;
using PetStuff.Catalog.Application.Features.Brands.Queries;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;

namespace PetStuff.Catalog.Application.Features.Brands.Handlers
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, List<BrandDto>>
    {
        private readonly IBrandRepository _repository;
        private readonly IMapper _mapper;

        public GetAllBrandsQueryHandler(IBrandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _repository.GetAllAsync();
            return _mapper.Map<List<BrandDto>>(brands);
        }
    }
}


