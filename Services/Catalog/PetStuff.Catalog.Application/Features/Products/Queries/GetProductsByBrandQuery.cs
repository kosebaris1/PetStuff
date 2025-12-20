using MediatR;
using PetStuff.Catalog.Application.DTOs;

namespace PetStuff.Catalog.Application.Features.Products.Queries
{
    public class GetProductsByBrandQuery : IRequest<List<ProductListDto>>
    {
        public int BrandId { get; set; }

        public GetProductsByBrandQuery(int brandId)
        {
            BrandId = brandId;
        }
    }
}
