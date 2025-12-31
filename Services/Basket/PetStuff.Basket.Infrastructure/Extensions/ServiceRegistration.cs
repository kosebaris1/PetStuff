using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetStuff.Basket.Application.Interfaces;
using PetStuff.Basket.Infrastructure.Repositories.BasketRepository;
using StackExchange.Redis;

namespace PetStuff.Basket.Infrastructure.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    Console.WriteLine($"Attempting to connect to Redis at: {redisConnectionString}");
                    
                    var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
                    configurationOptions.AbortOnConnectFail = false; // Bağlantı başarısız olsa bile devam et
                    configurationOptions.ConnectTimeout = 5000; // 5 saniye timeout
                    configurationOptions.SyncTimeout = 5000;
                    
                    var connection = ConnectionMultiplexer.Connect(configurationOptions);
                    
                    // Bağlantıyı test et
                    if (!connection.IsConnected)
                    {
                        throw new Exception("Redis connection failed - IsConnected is false");
                    }
                    
                    // Ping test
                    var database = connection.GetDatabase();
                    database.StringSet("test", "test");
                    var testValue = database.StringGet("test");
                    
                    Console.WriteLine("✅ Redis connection successful!");
                    
                    return connection;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Redis connection error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw new InvalidOperationException($"Failed to connect to Redis at {redisConnectionString}. Error: {ex.Message}", ex);
                }
            });

            services.AddScoped<IBasketRepository, BasketRepository>();

            return services;
        }
    }
}



