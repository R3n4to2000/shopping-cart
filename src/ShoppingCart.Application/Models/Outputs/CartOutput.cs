namespace ShoppingCart.Application.Models.Outputs;

public sealed record CartOutput(
    Guid Id,
    string Status,
    IReadOnlyCollection<CartItemOutput> Items,
    CouponOutput? AppliedCoupon,
    decimal Subtotal,
    decimal Discount,
    decimal Total);