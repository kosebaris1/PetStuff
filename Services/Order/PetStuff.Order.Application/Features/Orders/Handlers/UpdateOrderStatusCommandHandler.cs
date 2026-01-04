using MediatR;
using Microsoft.AspNetCore.Http;
using PetStuff.Order.Application.Features.Orders.Commands;
using PetStuff.Order.Application.Interfaces;
using OrderStatus = PetStuff.Order.Domain.Entities.OrderStatus;

namespace PetStuff.Order.Application.Features.Orders.Handlers
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICatalogServiceClient _catalogServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEventBus _eventBus;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            ICatalogServiceClient catalogServiceClient,
            IHttpContextAccessor httpContextAccessor,
            IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _catalogServiceClient = catalogServiceClient;
            _httpContextAccessor = httpContextAccessor;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Order'ı al
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return false;
            }

            // Eğer Pending -> Confirmed geçişiyse, stok kontrolü ve düşürme yap
            if (order.Status == OrderStatus.Pending && request.Status == OrderStatus.Confirmed)
            {
                // Token'ı al
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Authorization token not found.");
                }
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Stok kontrolü yap (Tekrar kontrol - başka biri almış olabilir)
                var stockCheckItems = order.Items.Select(item => new OrderItemStockRequest
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList();

                var stockAvailable = await _catalogServiceClient.CheckStockAsync(stockCheckItems, token);
                
                if (!stockAvailable)
                {
                    // Stok yetersizse, siparişi iptal et
                    await _orderRepository.UpdateOrderStatusAsync(request.OrderId, OrderStatus.Cancelled);
                    throw new InvalidOperationException("Insufficient stock. Order has been cancelled.");
                }

                // Stok düşür
                var stockReduced = await _catalogServiceClient.ReduceStockAsync(stockCheckItems, token);
                
                if (!stockReduced)
                {
                    // Stok düşürme başarısız olursa, siparişi iptal et
                    await _orderRepository.UpdateOrderStatusAsync(request.OrderId, OrderStatus.Cancelled);
                    throw new InvalidOperationException("Failed to reduce stock. Order has been cancelled.");
                }
            }

            // Status'u güncelle
            var result = await _orderRepository.UpdateOrderStatusAsync(request.OrderId, request.Status);

            // Eğer status güncellendi ve email gönderilmesi gereken durumlardan biri ise, RabbitMQ'ya mesaj gönder
            if (result && (request.Status == OrderStatus.Confirmed || request.Status == OrderStatus.Shipped || request.Status == OrderStatus.Delivered))
            {
                _eventBus.PublishOrderStatusChangedEvent(request.OrderId, order.UserId, request.Status.ToString());
            }

            return result;
        }
    }
}

