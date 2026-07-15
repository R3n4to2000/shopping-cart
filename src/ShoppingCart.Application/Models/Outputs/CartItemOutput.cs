namespace ShoppingCart.Application.Models.Outputs;

public sealed record CartItemOutput(
    Guid Id,
    int ProductId,
    string ProductDescription,
    int Quantity,
    int AvailableStock,
    decimal NetUnitPrice,
    decimal TotalPrice);