namespace ShoppingCart.Application.Models.Outputs;

public sealed record ProductOutput(
    int Id,
    string Description,
    int AvailableStock,
    decimal NetPrice);