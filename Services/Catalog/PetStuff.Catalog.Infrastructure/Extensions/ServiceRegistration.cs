using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;
using PetStuff.Catalog.Infrastructure.Context;
using PetStuff.Catalog.Infrastructure.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Infrastructure.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
