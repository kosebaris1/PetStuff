using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public int Id { get; set; }

        public GetProductByIdQuery(int id)
        {
            Id = id;
        }
    }
}
