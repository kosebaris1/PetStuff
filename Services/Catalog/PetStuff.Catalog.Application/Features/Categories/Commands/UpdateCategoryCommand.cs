using MediatR;

namespace PetStuff.Catalog.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}




