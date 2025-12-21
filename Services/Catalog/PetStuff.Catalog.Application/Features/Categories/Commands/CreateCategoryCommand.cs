using MediatR;

namespace PetStuff.Catalog.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<Unit>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}


