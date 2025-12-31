using System.Collections.Generic;

namespace PetStuff.Order.Application.DTOs
{
    public class BasketServiceDto
    {
        public string UserId { get; set; } = default!;
        public List<BasketItemServiceDto> Items { get; set; } = new List<BasketItemServiceDto>();
        public decimal TotalPrice { get; set; }
    }

    public class BasketItemServiceDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}



