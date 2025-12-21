using System.Threading.Tasks;
using BasketEntity = PetStuff.Basket.Domain.Entities.Basket;

namespace PetStuff.Basket.Application.Interfaces
{
    public interface IBasketRepository
    {
        Task<BasketEntity?> GetBasketAsync(string userId);
        Task<BasketEntity> UpdateBasketAsync(BasketEntity basket);
        Task<bool> DeleteBasketAsync(string userId);
    }
}

