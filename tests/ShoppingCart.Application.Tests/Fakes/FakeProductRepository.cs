using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace Application.Tests.Fakes;

internal sealed class FakeProductRepository
    : IProductRepository
{
    private readonly Dictionary<int, Product> _products;

    public FakeProductRepository(
        params Product[] products)
    {
        _products = products.ToDictionary(
            product => product.Id);
    }

    public Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        _products.TryGetValue(
            productId,
            out var product);

        return Task.FromResult(product);
    }

    public Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Product> products =
            _products.Values.ToArray();

        return Task.FromResult(products);
    }
}