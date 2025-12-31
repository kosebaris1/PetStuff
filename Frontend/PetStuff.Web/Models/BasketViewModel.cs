namespace PetStuff.Web.Models
{
    public class BasketViewModel
    {
        public string UserId { get; set; } = default!;
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public decimal TotalPrice { get; set; }
    }

    public class BasketItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ProductImageUrl { get; set; }
    }

    public class AddToBasketViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string? ProductImageUrl { get; set; }
    }
}

