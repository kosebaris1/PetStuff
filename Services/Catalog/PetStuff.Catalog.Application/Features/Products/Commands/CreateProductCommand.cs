using MediatR;
using PetStuff.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Unit>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }

        public int BrandId { get; set; }

        public List<string>? ImageUrls { get; set; }
    }
}
