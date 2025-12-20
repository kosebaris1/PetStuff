using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PetStuff.Catalog.Application.Behaviors;

namespace PetStuff.Catalog.Application.Registration
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ApplicationServiceRegistration).Assembly);
            
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

            return services;
        }
    }
}
