using MediatR;
using PetStuff.Order.Application.DTOs;

namespace PetStuff.Order.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        // UserId ve Items token'dan ve Basket Service'den otomatik alınacak
        public string ShippingAddress { get; set; } = default!;
        public string? ShippingCity { get; set; }
        public string? ShippingCountry { get; set; }
        public string? ShippingZipCode { get; set; }
    }
}

