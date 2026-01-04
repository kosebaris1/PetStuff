namespace PetStuff.Web.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ProductCount { get; set; }
    }

    public class CreateCategoryViewModel
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }

    public class UpdateCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
