using PetStuff.Web.Models;

namespace PetStuff.Web.Services
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetOrdersAsync(string token);
        Task<List<OrderViewModel>> GetAllOrdersAsync(string token); // Admin için
        Task<OrderViewModel?> GetOrderByIdAsync(int id, string token);
        Task<OrderViewModel?> CreateOrderAsync(CreateOrderViewModel order, string token);
        Task<bool> UpdateOrderStatusAsync(int id, string status, string token);
    }
}

