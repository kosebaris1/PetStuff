using PetStuff.Order.Application.DTOs;
using System.Threading.Tasks;

namespace PetStuff.Order.Application.Interfaces
{
    public interface IBasketServiceClient
    {
        Task<BasketServiceDto?> GetBasketAsync(string userId, string token);
        Task<bool> ClearBasketAsync(string userId, string token);
    }
}

