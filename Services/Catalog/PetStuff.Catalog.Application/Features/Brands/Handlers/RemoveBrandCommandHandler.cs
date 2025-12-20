using MediatR;
using PetStuff.Catalog.Application.Features.Brands.Commands;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;

namespace PetStuff.Catalog.Application.Features.Brands.Handlers
{
    public class RemoveBrandCommandHandler : IRequestHandler<RemoveBrandCommand, bool>
    {
        private readonly IBrandRepository _repository;

        public RemoveBrandCommandHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemoveBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repository.GetByIdAsync(request.Id);
            if (brand == null) return false;

            await _repository.DeleteBrandAsync(brand);
            return true;
        }
    }
}
