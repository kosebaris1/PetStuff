using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.DTOs;
using PetStuff.Catalog.Application.Features.Brands.Queries;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;

namespace PetStuff.Catalog.Application.Features.Brands.Handlers
{
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDto?>
    {
        private readonly IBrandRepository _repository;
        private readonly IMapper _mapper;

        public GetBrandByIdQueryHandler(IBrandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BrandDto?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var brand = await _repository.GetByIdAsync(request.Id);
            if (brand == null) return null;
            return _mapper.Map<BrandDto>(brand);
        }
    }
}


