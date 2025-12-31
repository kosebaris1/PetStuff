using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.DTOs;
using PetStuff.Catalog.Application.Features.Categories.Queries;
using PetStuff.Catalog.Application.Interfaces.CategoryInterface;

namespace PetStuff.Catalog.Application.Features.Categories.Handlers
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null) return null;
            return _mapper.Map<CategoryDto>(category);
        }
    }
}




