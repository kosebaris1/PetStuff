using PetStuff.Basket.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;
using BasketEntity = PetStuff.Basket.Domain.Entities.Basket;

namespace PetStuff.Basket.Infrastructure.Repositories.BasketRepository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        private const string BasketKeyPrefix = "basket:";

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<BasketEntity?> GetBasketAsync(string userId)
        {
            var key = $"{BasketKeyPrefix}{userId}";
            var data = await _database.StringGetAsync(key);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<BasketEntity>(data!);
        }

        public async Task<BasketEntity> UpdateBasketAsync(BasketEntity basket)
        {
            var key = $"{BasketKeyPrefix}{basket.UserId}";
            var json = JsonSerializer.Serialize(basket);

            await _database.StringSetAsync(key, json);

            return basket;
        }

        public async Task<bool> DeleteBasketAsync(string userId)
        {
            var key = $"{BasketKeyPrefix}{userId}";
            return await _database.KeyDeleteAsync(key);
        }
    }
}

