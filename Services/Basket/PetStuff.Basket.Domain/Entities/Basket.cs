using System.Collections.Generic;
using System.Linq;

namespace PetStuff.Basket.Domain.Entities
{
    public class Basket
    {
        public string UserId { get; set; } = default!;
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public decimal TotalPrice
        {
            get
            {
                return Items?.Sum(item => item.Price * item.Quantity) ?? 0;
            }
        }
    }
}

