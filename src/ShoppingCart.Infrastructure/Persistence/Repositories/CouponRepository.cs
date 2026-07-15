using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Persistence.Repositories;

public sealed class CouponRepository
    : ICouponRepository
{
    private readonly ShoppingCartDbContext _dbContext;

    public CouponRepository(
        ShoppingCartDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(
                nameof(dbContext));
    }

    public async Task<Coupon?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var normalizedCode = code
            .Trim()
            .ToUpperInvariant();

        return await _dbContext.Coupons
            .SingleOrDefaultAsync(
                coupon => coupon.Code == normalizedCode,
                cancellationToken);
    }
}