using MediatR;
using PetStuff.Catalog.Application.Features.Categories.Commands;
using PetStuff.Catalog.Application.Interfaces.CategoryInterface;

namespace PetStuff.Catalog.Application.Features.Categories.Handlers
{
    public class RemoveCategoryCommandHandler : IRequestHandler<RemoveCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;

        public RemoveCategoryCommandHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null) return false;

            await _repository.DeleteCategoryAsync(category);
            return true;
        }
    }
}
