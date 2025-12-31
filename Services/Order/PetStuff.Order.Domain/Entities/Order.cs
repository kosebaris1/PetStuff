namespace PetStuff.Order.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } = default!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string ShippingAddress { get; set; } = default!;
        public string? ShippingCity { get; set; }
        public string? ShippingCountry { get; set; }
        public string? ShippingZipCode { get; set; }
        
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}



