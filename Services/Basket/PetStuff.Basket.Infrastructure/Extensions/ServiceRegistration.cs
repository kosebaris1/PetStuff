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
                var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            services.AddScoped<IBasketRepository, BasketRepository>();

            return services;
        }
    }
}



