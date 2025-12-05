using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
