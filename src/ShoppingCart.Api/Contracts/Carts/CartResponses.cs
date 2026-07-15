namespace ShoppingCart.Api.Contracts.Carts;

public sealed record CouponResponse(
    int Id,
    string Code,
    decimal DiscountPercentage);

public sealed record CartItemResponse(
    Guid Id,
    int ProductId,
    string ProductDescription,
    int Quantity,
    int AvailableStock,
    decimal NetUnitPrice,
    decimal TotalPrice);

public sealed record CartResponse(
    Guid Id,
    string Status,
    IReadOnlyCollection<CartItemResponse> Items,
    CouponResponse? AppliedCoupon,
    decimal Subtotal,
    decimal Discount,
    decimal Total);