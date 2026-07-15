using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Abstractions.Persistence;

public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Cart cart,
        CancellationToken cancellationToken = default);
}