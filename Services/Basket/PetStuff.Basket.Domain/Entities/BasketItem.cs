namespace PetStuff.Basket.Domain.Entities
{
    public class BasketItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}



