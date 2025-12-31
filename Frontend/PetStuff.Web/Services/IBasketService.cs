using PetStuff.Web.Models;

namespace PetStuff.Web.Services
{
    public interface IBasketService
    {
        Task<BasketViewModel?> GetBasketAsync(string token);
        Task<bool> AddItemToBasketAsync(AddToBasketViewModel item, string token);
        Task<bool> UpdateItemQuantityAsync(int productId, int quantity, string token);
        Task<bool> RemoveItemFromBasketAsync(int productId, string token);
        Task<bool> ClearBasketAsync(string token);
    }
}

