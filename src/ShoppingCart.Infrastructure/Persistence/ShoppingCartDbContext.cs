using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Persistence;

public sealed class ShoppingCartDbContext
    : DbContext,
      IUnitOfWork
{
    public ShoppingCartDbContext(
        DbContextOptions<ShoppingCartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products =>
        Set<Product>();

    public DbSet<Coupon> Coupons =>
        Set<Coupon>();

    public DbSet<Cart> Carts =>
        Set<Cart>();

    public DbSet<CartItem> CartItems =>
        Set<CartItem>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ShoppingCartDbContext).Assembly);
    }
}