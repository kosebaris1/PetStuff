namespace PetStuff.Catalog.Application.DTOs
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = default!;
        public string BrandName { get; set; } = default!;
        public string? MainImageUrl { get; set; }
    }
}




