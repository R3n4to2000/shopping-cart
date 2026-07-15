using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);
}