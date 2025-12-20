namespace PetStuff.Catalog.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public CategoryDto Category { get; set; } = default!;
        public BrandDto Brand { get; set; } = default!;
        public List<string> ImageUrls { get; set; } = new();
    }
}
