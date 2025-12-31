using PetStuff.Web.Models;

namespace PetStuff.Web.Services
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetOrdersAsync(string token);
        Task<OrderViewModel?> GetOrderByIdAsync(int id, string token);
        Task<OrderViewModel?> CreateOrderAsync(CreateOrderViewModel order, string token);
    }
}

