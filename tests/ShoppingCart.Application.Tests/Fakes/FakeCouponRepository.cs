using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Domain.Entities;

namespace Application.Tests.Fakes;

internal sealed class FakeCouponRepository
    : ICouponRepository
{
    private readonly Dictionary<string, Coupon> _coupons;

    public FakeCouponRepository(
        params Coupon[] coupons)
    {
        _coupons = coupons.ToDictionary(
            coupon => coupon.Code,
            StringComparer.OrdinalIgnoreCase);
    }

    public Task<Coupon?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        _coupons.TryGetValue(
            code,
            out var coupon);

        return Task.FromResult(coupon);
    }
}