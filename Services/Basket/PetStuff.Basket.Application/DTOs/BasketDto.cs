using PetStuff.Basket.Domain.Entities;
using System.Collections.Generic;

namespace PetStuff.Basket.Application.DTOs
{
    public class BasketDto
    {
        public string UserId { get; set; } = default!;
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        public decimal TotalPrice { get; set; }
    }
}

