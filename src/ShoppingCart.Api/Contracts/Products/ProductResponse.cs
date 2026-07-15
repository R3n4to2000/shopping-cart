namespace ShoppingCart.Api.Contracts.Products;

public sealed record ProductResponse(
    int Id,
    string Description,
    int AvailableStock,
    decimal NetPrice);