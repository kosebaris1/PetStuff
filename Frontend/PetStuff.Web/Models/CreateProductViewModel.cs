using System.ComponentModel.DataAnnotations;

namespace PetStuff.Web.Models
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BrandId { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateProductViewModel
    {
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BrandId { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}

