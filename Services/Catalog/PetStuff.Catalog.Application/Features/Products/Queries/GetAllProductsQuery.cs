using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Products.Queries
{
    public class GetAllProductsQuery : IRequest<List<ProductListDto>>
    {
    }
}
