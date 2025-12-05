using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; } = default!;
        public bool IsMain { get; set; } = false; 

        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
