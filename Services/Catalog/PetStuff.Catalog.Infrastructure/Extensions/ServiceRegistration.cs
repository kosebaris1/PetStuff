using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;
using PetStuff.Catalog.Application.Interfaces.CategoryInterface;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;
using PetStuff.Catalog.Infrastructure.Context;
using PetStuff.Catalog.Infrastructure.Repositories.BrandRepository;
using PetStuff.Catalog.Infrastructure.Repositories.CategoryRepository;
using PetStuff.Catalog.Infrastructure.Repositories.ProductRepository;

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
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();

            return services;
        }
    }
}
