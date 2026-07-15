using ShoppingCart.Domain.Entities;
using ShoppingCart.Infrastructure.Persistence.Seed.Models;

namespace ShoppingCart.Infrastructure.Persistence.Seed;

internal static class CatalogSeed
{
    private static readonly Lazy<Product[]> ProductsData =
        new(CreateProducts);

    private static readonly Lazy<Coupon[]> CouponsData =
        new(CreateCoupons);

    internal static Product[] Products =>
        ProductsData.Value;

    internal static Coupon[] Coupons =>
        CouponsData.Value;

    private static Product[] CreateProducts()
    {
        return SeedDataLoader
            .Load<ProductSeedModel>("catalogoProdutos.json")
            .Select(item => new Product(
                id: item.Id,
                description: item.Description,
                stockQuantity: item.StockQuantity,
                netPrice: item.NetPrice))
            .ToArray();
    }

    private static Coupon[] CreateCoupons()
    {
        return SeedDataLoader
            .Load<CouponSeedModel>("cupons.json")
            .Select(item => new Coupon(
                id: item.Id,
                code: item.Code,
                discountPercentage: item.DiscountPercentage))
            .ToArray();
    }
}