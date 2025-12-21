using MediatR;

namespace PetStuff.Catalog.Application.Features.Brands.Commands
{
    public class UpdateBrandCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}


