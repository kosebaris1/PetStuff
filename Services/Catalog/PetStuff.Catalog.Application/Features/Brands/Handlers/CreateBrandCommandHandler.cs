using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.Features.Brands.Commands;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;
using PetStuff.Catalog.Domain.Entities;

namespace PetStuff.Catalog.Application.Features.Brands.Handlers
{
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand>
    {
        private readonly IBrandRepository _repository;
        private readonly IMapper _mapper;

        public CreateBrandCommandHandler(IBrandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = _mapper.Map<Brand>(request);
            await _repository.CreateBrandAsync(brand);
            return Unit.Value;
        }
    }
}


