namespace PetStuff.Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public CategoryViewModel? Category { get; set; }
        public BrandViewModel? Brand { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public string? MainImageUrl { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class BrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}

