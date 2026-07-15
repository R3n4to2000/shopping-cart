using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Infrastructure.Persistence;
using ShoppingCart.Infrastructure.Persistence.Repositories;

namespace ShoppingCart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada.");
        }

        services.AddDbContext<ShoppingCartDbContext>(
            options =>
            {
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly(
                            typeof(ShoppingCartDbContext)
                                .Assembly
                                .FullName);
                    });
            });

        services.AddScoped<
            IProductRepository,
            ProductRepository>();

        services.AddScoped<
            ICouponRepository,
            CouponRepository>();

        services.AddScoped<
            ICartRepository,
            CartRepository>();

        services.AddScoped<IUnitOfWork>(
            serviceProvider =>
                serviceProvider.GetRequiredService<
                    ShoppingCartDbContext>());

        return services;
    }
}