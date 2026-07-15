namespace ShoppingCart.Application.Models.Inputs;

public sealed record GetCartInput(
    Guid CartId);

public sealed record AddProductToCartInput(
    Guid CartId,
    int ProductId,
    int Quantity);

public sealed record ChangeCartItemQuantityInput(
    Guid CartId,
    int ProductId,
    int Quantity);

public sealed record RemoveProductFromCartInput(
    Guid CartId,
    int ProductId);

public sealed record ApplyCouponInput(
    Guid CartId,
    string Code);

public sealed record RemoveCouponInput(
    Guid CartId);

public sealed record CheckoutCartInput(
    Guid CartId);