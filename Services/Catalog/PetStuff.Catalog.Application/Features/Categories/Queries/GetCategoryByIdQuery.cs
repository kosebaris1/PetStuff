using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public int Id { get; set; }

        public GetCategoryByIdQuery(int id)
        {
            Id = id;
        }
    }
}
