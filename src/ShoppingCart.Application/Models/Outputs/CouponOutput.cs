namespace ShoppingCart.Application.Models.Outputs;

public sealed record CouponOutput(
    int Id,
    string Code,
    decimal DiscountPercentage);