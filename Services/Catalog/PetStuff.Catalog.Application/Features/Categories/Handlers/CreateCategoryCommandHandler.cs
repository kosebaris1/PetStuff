using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.Features.Categories.Commands;
using PetStuff.Catalog.Application.Interfaces.CategoryInterface;
using PetStuff.Catalog.Domain.Entities;

namespace PetStuff.Catalog.Application.Features.Categories.Handlers
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(request);
            await _repository.CreateCategoryAsync(category);
            return Unit.Value;
        }
    }
}
