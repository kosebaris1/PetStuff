namespace PetStuff.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = default!;
        public string ShippingAddress { get; set; } = default!;
        public string? ShippingCity { get; set; }
        public string? ShippingCountry { get; set; }
        public string? ShippingZipCode { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ProductImageUrl { get; set; }
    }

    public class CreateOrderViewModel
    {
        public string ShippingAddress { get; set; } = default!;
        public string? ShippingCity { get; set; }
        public string? ShippingCountry { get; set; }
        public string? ShippingZipCode { get; set; }
    }
}

