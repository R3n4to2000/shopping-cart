using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Abstractions.Persistence;

public interface ICouponRepository
{
	Task<Coupon?> GetByCodeAsync(
		string code,
		CancellationToken cancellationToken = default);
}