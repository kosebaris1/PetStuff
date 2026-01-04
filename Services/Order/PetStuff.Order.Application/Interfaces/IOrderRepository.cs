using System.Collections.Generic;
using System.Threading.Tasks;
using OrderEntity = PetStuff.Order.Domain.Entities.Order;
using OrderStatus = PetStuff.Order.Domain.Entities.OrderStatus;

namespace PetStuff.Order.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderEntity?> GetByIdAsync(int id);
        Task<List<OrderEntity>> GetByUserIdAsync(string userId);
        Task<List<OrderEntity>> GetAllAsync();
        Task<OrderEntity> CreateOrderAsync(OrderEntity order);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}

