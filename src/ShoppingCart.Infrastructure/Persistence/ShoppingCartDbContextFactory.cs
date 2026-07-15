using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoppingCart.Infrastructure.Persistence;

public sealed class ShoppingCartDbContextFactory
    : IDesignTimeDbContextFactory<ShoppingCartDbContext>
{
    public ShoppingCartDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure a variável de ambiente " +
                "'ConnectionStrings__DefaultConnection' " +
                "antes de executar os comandos do Entity Framework.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<
                ShoppingCartDbContext>();

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(
                    typeof(ShoppingCartDbContext)
                        .Assembly
                        .FullName);
            });

        return new ShoppingCartDbContext(
            optionsBuilder.Options);
    }
}