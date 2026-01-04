namespace PetStuff.Web.Models
{
    public class BrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ProductCount { get; set; }
    }

    public class CreateBrandViewModel
    {
        public string Name { get; set; } = default!;
    }

    public class UpdateBrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
