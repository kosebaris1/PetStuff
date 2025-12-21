using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Brands.Queries
{
    public class GetAllBrandsQuery : IRequest<List<BrandDto>>
    {
    }
}


