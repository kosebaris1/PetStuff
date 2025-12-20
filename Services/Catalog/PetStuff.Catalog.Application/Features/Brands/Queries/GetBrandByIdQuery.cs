using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Brands.Queries
{
    public class GetBrandByIdQuery : IRequest<BrandDto?>
    {
        public int Id { get; set; }

        public GetBrandByIdQuery(int id)
        {
            Id = id;
        }
    }
}
