using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using PetStuff.Order.Application.DTOs;
using PetStuff.Order.Application.Features.Orders.Commands;
using PetStuff.Order.Application.Interfaces;
using System.Linq;
using System.Security.Claims;
using OrderEntity = PetStuff.Order.Domain.Entities.Order;
using OrderItemEntity = PetStuff.Order.Domain.Entities.OrderItem;
using OrderStatus = PetStuff.Order.Domain.Entities.OrderStatus;

namespace PetStuff.Order.Application.Features.Orders.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketServiceClient _basketServiceClient;
        private readonly ICatalogServiceClient _catalogServiceClient;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IBasketServiceClient basketServiceClient,
            ICatalogServiceClient catalogServiceClient,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _basketServiceClient = basketServiceClient;
            _catalogServiceClient = catalogServiceClient;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Token'dan userId al
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier) ??
                _httpContextAccessor.HttpContext?.User
                .FindFirst("sub");
            
            var userId = userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token.");

            // 2. Token'ı al
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Authorization token not found.");
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();

            // 3. Basket Service'den sepet bilgisi al
            var basket = await _basketServiceClient.GetBasketAsync(userId, token);

            if (basket == null || basket.Items == null || !basket.Items.Any())
            {
                throw new InvalidOperationException("Basket is empty or could not be retrieved.");
            }

            // 4. Stok kontrolü yap (SADECE KONTROL, düşürme YOK)
            var stockCheckItems = basket.Items.Select(item => new OrderItemStockRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            var stockAvailable = await _catalogServiceClient.CheckStockAsync(stockCheckItems, token);
            
            if (!stockAvailable)
            {
                throw new InvalidOperationException("Insufficient stock for one or more products in the basket.");
            }

            // 5. Order oluştur (Status: Pending - Henüz onaylanmadı, stok düşürülmedi)
            var order = new OrderEntity
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity,
                ShippingCountry = request.ShippingCountry,
                ShippingZipCode = request.ShippingZipCode,
                Status = OrderStatus.Pending, // Pending - Admin onayı bekliyor
                Items = basket.Items.Select(item => new OrderItemEntity
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductImageUrl = item.ProductImageUrl
                }).ToList()
            };

            // Total price hesapla
            order.TotalPrice = order.Items.Sum(item => item.Price * item.Quantity);

            // 6. Order'ı kaydet (Stok düşürülmedi, sadece kaydedildi)
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // 7. Basket Service'e sepeti temizlemesini söyle
            await _basketServiceClient.ClearBasketAsync(userId, token);

            return _mapper.Map<OrderDto>(createdOrder);
        }
    }
}

