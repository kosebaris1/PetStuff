using MediatR;

namespace PetStuff.Catalog.Application.Features.Brands.Commands
{
    public class CreateBrandCommand : IRequest<Unit>
    {
        public string Name { get; set; } = default!;
    }
}




