using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetStuff.Order.Application.Interfaces;
using PetStuff.Order.Infrastructure.Context;
using PetStuff.Order.Infrastructure.Repositories.OrderRepository;
using PetStuff.Order.Infrastructure.Services;

namespace PetStuff.Order.Infrastructure.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrderDb");

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IOrderRepository, OrderRepository>();

            // HttpClient for Basket Service
            services.AddHttpClient<IBasketServiceClient, BasketServiceClient>();

            return services;
        }
    }
}

