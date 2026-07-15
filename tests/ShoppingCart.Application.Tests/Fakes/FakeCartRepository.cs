using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace Application.Tests.Fakes;

internal sealed class FakeCartRepository
    : ICartRepository
{
    private readonly Dictionary<Guid, Cart> _carts = [];

    public IReadOnlyCollection<Cart> Carts =>
        _carts.Values.ToArray();

    public Task<Cart?> GetByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default)
    {
        _carts.TryGetValue(
            cartId,
            out var cart);

        return Task.FromResult(cart);
    }

    public Task AddAsync(
        Cart cart,
        CancellationToken cancellationToken = default)
    {
        _carts.Add(
            cart.Id,
            cart);

        return Task.CompletedTask;
    }

    public void Seed(Cart cart)
    {
        _carts[cart.Id] = cart;
    }
}