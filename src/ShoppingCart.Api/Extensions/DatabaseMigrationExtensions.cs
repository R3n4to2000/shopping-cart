using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure.Persistence;

namespace ShoppingCart.Api.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(
        this WebApplication app)
    {
        var applyMigrations = app.Configuration.GetValue<bool>(
            "Database:ApplyMigrationsOnStartup");

        if (!applyMigrations)
        {
            return;
        }

        await using var scope =
            app.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ShoppingCartDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}