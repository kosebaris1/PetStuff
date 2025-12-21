using MediatR;

namespace PetStuff.Catalog.Application.Features.Brands.Commands
{
    public class RemoveBrandCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public RemoveBrandCommand(int id)
        {
            Id = id;
        }
    }
}


