using MediatR;

namespace PetStuff.Catalog.Application.Features.Categories.Commands
{
    public class RemoveCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public RemoveCategoryCommand(int id)
        {
            Id = id;
        }
    }
}




