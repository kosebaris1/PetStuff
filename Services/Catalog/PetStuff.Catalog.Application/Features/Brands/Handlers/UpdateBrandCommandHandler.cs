using MediatR;
using PetStuff.Catalog.Application.Features.Brands.Commands;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;

namespace PetStuff.Catalog.Application.Features.Brands.Handlers
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, bool>
    {
        private readonly IBrandRepository _repository;

        public UpdateBrandCommandHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repository.GetByIdAsync(request.Id);
            if (brand == null) return false;

            brand.Name = request.Name;
            brand.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateBrandAsync(brand);
            return true;
        }
    }
}


