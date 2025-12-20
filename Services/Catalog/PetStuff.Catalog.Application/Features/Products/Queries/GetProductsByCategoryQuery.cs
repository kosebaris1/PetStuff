using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Products.Queries
{
    public class GetProductsByCategoryQuery : IRequest<List<ProductListDto>>
    {
        public int CategoryId { get; set; }

        public GetProductsByCategoryQuery(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
