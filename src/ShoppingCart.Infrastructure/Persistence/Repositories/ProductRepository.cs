using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository
    : IProductRepository
{
    private readonly ShoppingCartDbContext _dbContext;

    public ProductRepository(
        ShoppingCartDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(
                nameof(dbContext));
    }

    public async Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .SingleOrDefaultAsync(
                product => product.Id == productId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .ToArrayAsync(cancellationToken);
    }
}