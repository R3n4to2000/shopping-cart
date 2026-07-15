using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Persistence.Repositories;

public sealed class CartRepository
    : ICartRepository
{
    private readonly ShoppingCartDbContext _dbContext;

    public CartRepository(
        ShoppingCartDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(
                nameof(dbContext));
    }

    public async Task<Cart?> GetByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Carts
            .Include(cart => cart.Items)
                .ThenInclude(item => item.Product)
            .Include(cart => cart.AppliedCoupon)
            .SingleOrDefaultAsync(
                cart => cart.Id == cartId,
                cancellationToken);
    }

    public async Task AddAsync(
        Cart cart,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cart);

        await _dbContext.Carts.AddAsync(
            cart,
            cancellationToken);
    }
}