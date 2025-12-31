using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.Features.Categories.Commands;
using PetStuff.Catalog.Application.Interfaces.CategoryInterface;

namespace PetStuff.Catalog.Application.Features.Categories.Handlers
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null) return false;

            category.Name = request.Name;
            category.Description = request.Description;
            category.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateCategoryAsync(category);
            return true;
        }
    }
}




