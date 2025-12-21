using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetStuff.IdentityServer.Configuration;
using PetStuff.IdentityServer.Data;
using PetStuff.IdentityServer.Models;
using PetStuff.IdentityServer.Services;
using ConfigurationDbContext = Duende.IdentityServer.EntityFramework.DbContexts.ConfigurationDbContext;
using PersistedGrantDbContext = Duende.IdentityServer.EntityFramework.DbContexts.PersistedGrantDbContext;

namespace PetStuff.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();

            // Identity Server DbContext
            builder.Services.AddDbContext<IdentityServerDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            // ASP.NET Core Identity
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityServerDbContext>()
            .AddDefaultTokenProviders();

            // Profile Service for adding roles to token
            builder.Services.AddScoped<IProfileService, ProfileService>();

            // Identity Server
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.EmitStaticAudienceClaim = true;
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbContextOptions =>
                        dbContextOptions.UseSqlServer(connectionString, opt =>
                            opt.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbContextOptions =>
                        dbContextOptions.UseSqlServer(connectionString, opt =>
                            opt.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<User>()
                .AddDeveloperSigningCredential(); // Sadece development için, production'da certificate kullanılmalı

            var app = builder.Build();

            // Seed IdentityServer Configuration Data
            InitializeDatabase(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                // Identity Server Configuration DbContext
                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityServerConfiguration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityServerConfiguration.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in IdentityServerConfiguration.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    context.SaveChanges();
                }

                // API Resources - Add new or update UserClaims
                var existingApiResources = context.ApiResources.Include(r => r.UserClaims).ToList();
                var configApiResources = IdentityServerConfiguration.GetApiResources().ToList();

                foreach (var configResource in configApiResources)
                {
                    var existingResource = existingApiResources.FirstOrDefault(r => r.Name == configResource.Name);
                    if (existingResource == null)
                    {
                        // Add new resource
                        context.ApiResources.Add(configResource.ToEntity());
                    }
                    else
                    {
                        // Update UserClaims - Remove old ones and add new ones
                        var oldClaims = existingResource.UserClaims?.ToList() ?? new List<Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim>();
                        context.Set<Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim>().RemoveRange(oldClaims);

                        foreach (var claim in configResource.UserClaims)
                        {
                            context.Set<Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim>()
                                .Add(new Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim
                                {
                                    ApiResourceId = existingResource.Id,
                                    Type = claim
                                });
                        }
                    }
                }
                context.SaveChanges();

                // Persisted Grant DbContext
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            }
        }
    }
}
